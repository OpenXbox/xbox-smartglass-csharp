using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartGlass.Common;
using SmartGlass.Messaging.Session.Messages;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading;

namespace SmartGlass.Messaging.Session
{
    internal class SessionMessageTransport : IDisposable, IMessageTransport<SessionMessageBase>
    {
        // TODO: Decide on severities
        private static readonly ILogger logger = Logging.Factory.CreateLogger<SessionMessageTransport>();

        private static readonly TimeSpan[] messageRetries = new TimeSpan[]
        {
            TimeSpan.FromMilliseconds(500),
            TimeSpan.FromMilliseconds(500),
            TimeSpan.FromMilliseconds(1500),
            TimeSpan.FromMilliseconds(1500),
            TimeSpan.FromMilliseconds(3500),
            TimeSpan.FromMilliseconds(5000)
        };

        public static SessionMessageBase CreateFromMessageType(SessionMessageType messageType)
        {
            var type = SessionMessageTypeAttribute.GetTypeForMessageType(messageType);
            if (type == null)
            {
                return new UnknownMessage();
            }

            return (SessionMessageBase)Activator.CreateInstance(type);
        }

        private readonly int _heartbeatTimeoutSeconds;
        private readonly object _lockObject = new object();
        private readonly CancellationTokenSource _cancellationTokenSource;

        private readonly MessageTransport _transport;
        private readonly uint _participantId;

        private readonly FragmentMessageManager _fragment_manager;

        private DateTime _lastReceived;
        private uint _sequenceNumber;

        private uint _serverSequenceNumber;

        private bool _isDisposed;

        public event EventHandler<MessageReceivedEventArgs<SessionMessageBase>> MessageReceived;
        public event EventHandler<EventArgs> ProtocolTimeoutOccured;

        public SessionMessageTransport(
            MessageTransport transport,
            SessionInfo sessionInfo,
            int heartbeatTimeoutSeconds = 3)
        {
            _transport = transport;

            _participantId = sessionInfo.ParticipantId;

            _heartbeatTimeoutSeconds = heartbeatTimeoutSeconds;

            _transport.MessageReceived += TransportMessageReceived;

            _fragment_manager = new FragmentMessageManager();
            _lastReceived = DateTime.Now;

            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void StartHeartbeat()
        {
            Task.Run(() =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        SendHeartbeatAsync().GetAwaiter().GetResult();
                    }
                    catch (TimeoutException)
                    {
                        ProtocolTimeoutOccured?.Invoke(this, new EventArgs());
                        break;
                    }
                    Task.Delay(TimeSpan.FromSeconds(_heartbeatTimeoutSeconds))
                        .GetAwaiter().GetResult();
                }
            }, _cancellationTokenSource.Token);
        }

        private void TransportMessageReceived(object sender, MessageReceivedEventArgs<IMessage> e)
        {
            var fragmentMessage = e.Message as SessionFragmentMessage;
            if (fragmentMessage == null)
            {
                return;
            }

            if (fragmentMessage.InvalidSignature)
            {
                logger.LogWarning("Message has invalid signature. Ignoring...");
                return;
            }

            if (fragmentMessage.Header.TargetParticipantId != _participantId)
            {
                logger.LogWarning("Message has invalid participant id. Ignoring...");
                return;
            }

            var message = DeserializeMessage(fragmentMessage);

            logger.LogTrace($"Received message #{fragmentMessage.Header.SequenceNumber} ({message.ToString()})");

            if (message.Header.RequestAcknowledge)
            {
                SendMessageAckAsync(new uint[] { fragmentMessage.Header.SequenceNumber })
                    .GetAwaiter().GetResult();
            }

            if (fragmentMessage.Header.SequenceNumber <= _serverSequenceNumber)
            {
                // TODO: Make sure messages don't get lost incorrectly.
                logger.LogDebug("Message is too old. Ignoring...");
                return;
            }

            _lastReceived = DateTime.Now;
            _serverSequenceNumber = fragmentMessage.Header.SequenceNumber;

            if (fragmentMessage.Header.IsFragment)
            {
                message = _fragment_manager.AssembleFragment(message, fragmentMessage.Header.SequenceNumber);
                if (message == null)
                {
                    Debug.WriteLine($"FragmentMessage {message.Header.SessionMessageType} not ready yet");
                    return;
                }
            }

            MessageReceived?.Invoke(this, new MessageReceivedEventArgs<SessionMessageBase>(message));
        }

        // TODO: When to use reject?
        private Task SendMessageAckAsync(uint[] processed = null, uint[] rejected = null,
                                                                  bool requestAck = false)
        {
            if (processed != null)
            {
                logger.LogTrace($"Acking #{String.Join(",", processed)}");
            }

            var ackMessage = new AckMessage();
            ackMessage.Header.RequestAcknowledge = requestAck;
            ackMessage.LowWatermark = _serverSequenceNumber;
            ackMessage.ProcessedList = new HashSet<uint>(processed == null ?
                                                            new uint[0] :
                                                            processed);
            ackMessage.RejectedList = new HashSet<uint>(rejected == null ?
                                                            new uint[0] :
                                                            rejected);
            return SendAsync(ackMessage);
        }

        private Task SendHeartbeatAsync()
        {
            return SendMessageAckAsync(requestAck: true);
        }

        private SessionMessageBase DeserializeMessage(SessionFragmentMessage fragment)
        {
            var type = SessionMessageTypeAttribute.GetTypeForMessageType(fragment.Header.SessionMessageType);
            if (type == null)
            {
                logger.LogTrace("Incoming decrypted has no impl: " +
                    JsonConvert.SerializeObject(fragment, Formatting.Indented));
            }

            var message = CreateFromMessageType(fragment.Header.SessionMessageType);
            if (fragment.Header.IsFragment)
            {
                message = new FragmentMessage();
            }

            logger.LogTrace($"Received {message.GetType().Name} message");

            message.Header.ChannelId = fragment.Header.ChannelId;
            message.Header.RequestAcknowledge = fragment.Header.RequestAcknowledge;
            message.Header.SessionMessageType = fragment.Header.SessionMessageType;
            message.Header.Version = fragment.Header.Version;

            message.Deserialize(new BEReader(fragment.Fragment));

            return message;
        }

        private Task SendFragmentAsync(SessionMessageBase message, uint sequenceNumber)
        {
            logger.LogTrace($"Sending {message.GetType().Name} message");

            var fragment = new SessionFragmentMessage();

            fragment.Header.ChannelId = message.Header.ChannelId;
            fragment.Header.RequestAcknowledge = message.Header.RequestAcknowledge;
            fragment.Header.SessionMessageType = message.Header.SessionMessageType;
            fragment.Header.Version = message.Header.Version;

            fragment.Header.SequenceNumber = sequenceNumber;
            fragment.Header.SourceParticipantId = _participantId;

            var writer = new BEWriter();
            message.Serialize(writer);
            fragment.Fragment = writer.ToBytes();

            return _transport.SendAsync(fragment);
        }

        public Task SendAsync(SessionMessageBase message)
        {
            lock (_lockObject)
            {
                _sequenceNumber = _sequenceNumber + 1;
                var sequenceNumber = _sequenceNumber;

                logger.LogTrace($"Sending outbound #{sequenceNumber}...");

                if (message.Header.RequestAcknowledge)
                {
                    return TaskExtensions.WithRetries(async () =>
                    {
                        var ackMessage = await WaitForMessageAsync<AckMessage>(
                            TimeSpan.FromSeconds(1),
                            () => SendFragmentAsync(message, sequenceNumber),
                            ack => ack.ProcessedList.Contains(sequenceNumber) ||
                                   ack.RejectedList.Contains(sequenceNumber));

                        if (ackMessage.RejectedList.Contains(sequenceNumber))
                        {
                            throw new SmartGlassException("Message rejected by server.");
                        }

                        logger.LogTrace($"Got ack for outbound #{sequenceNumber}");
                    },
                    messageRetries);
                }
                else
                {
                    return SendFragmentAsync(message, sequenceNumber);
                }
            }
        }

        public Task<SessionMessageBase> WaitForMessageAsync(TimeSpan timeout, Action startAction)
        {
            return this.WaitForMessageAsync<SessionMessageBase, SessionMessageBase>(timeout, startAction);
        }

        public Task<T> WaitForMessageAsync<T>(TimeSpan timeout, Action startAction, Func<T, bool> filter = null)
            where T : SessionMessageBase
        {
            return this.WaitForMessageAsync<T, SessionMessageBase>(timeout, startAction, filter);
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            try
            {
                SendAsync(new DisconnectMessage())
                    .GetAwaiter().GetResult();
            }
            catch
            {
                // TODO: Trace
            }

            _cancellationTokenSource.Cancel();
            _transport.MessageReceived -= TransportMessageReceived;
        }
    }
}
