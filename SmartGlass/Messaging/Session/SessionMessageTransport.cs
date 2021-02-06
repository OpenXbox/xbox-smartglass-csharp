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
    /// <summary>
    /// Session message transport.
    /// </summary>
    internal class SessionMessageTransport : IDisposable, IMessageTransport<SessionMessageBase>
    {
        private bool _disposed = false;
        // TODO: Decide on severities
        private static readonly ILogger logger = Logging.Factory.CreateLogger<SessionMessageTransport>();

        private static readonly TimeSpan[] messageRetries = new TimeSpan[]
        {
            TimeSpan.FromMilliseconds(300),
            TimeSpan.FromMilliseconds(500)
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

        private readonly object _lockObject = new object();
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly TimeSpan _heartbeatTimeout;
        private readonly TimeSpan _heartbeatInterval;

        private readonly MessageTransport _transport;
        private readonly uint _participantId;

        private readonly FragmentMessageManager _fragmentManager;
        private readonly JsonFragmentManager _jsonFragmentManager;

        private DateTime _lastReceived;
        private uint _sequenceNumber;

        private uint _serverSequenceNumber;

        public event EventHandler<MessageReceivedEventArgs<SessionMessageBase>> MessageReceived;
        public event EventHandler<EventArgs> OnProtocolTimeout;

        public SessionMessageTransport(
            MessageTransport transport,
            SessionInfo sessionInfo,
            int heartbeatTimeoutSeconds = 10)
        {
            _transport = transport;

            _participantId = sessionInfo.ParticipantId;

            _heartbeatInterval = TimeSpan.FromSeconds(3);
            _heartbeatTimeout = TimeSpan.FromSeconds(heartbeatTimeoutSeconds);

            _transport.MessageReceived += TransportMessageReceived;

            _lastReceived = DateTime.Now;

            _fragmentManager = new FragmentMessageManager();
            _jsonFragmentManager = new JsonFragmentManager();

            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void StartHeartbeat()
        {
            Task.Run(async () =>
            {
                _lastReceived = DateTime.Now;
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        await SendMessageAckAsync(requestAck: true);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError("Error sending heartbeat", ex);
                    }
                    await Task.Delay(_heartbeatInterval);
                    if (DateTime.Now - _lastReceived > _heartbeatTimeout)
                    {
                        OnProtocolTimeout?.Invoke(this, new EventArgs());
                        break;
                    }
                }
            }, _cancellationTokenSource.Token);
        }

        private void TransportMessageReceived(object sender, MessageReceivedEventArgs<IMessage> e)
        {
            var fragmentMessage = e.Message as SessionFragmentMessage;
            if (fragmentMessage == null)
            {
                logger.LogWarning($"Ignoring non fragment: {e.Message}");
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

            logger.LogTrace($"Received message #{fragmentMessage.Header.SequenceNumber} ({message})");

            if (message.Header.RequestAcknowledge)
            {
                SendMessageAckAsync(new uint[] { fragmentMessage.Header.SequenceNumber })
                    .Wait();
            }

            /*
            if (fragmentMessage.Header.SequenceNumber <= _serverSequenceNumber)
            {
                // TODO: Make sure messages don't get lost incorrectly.
                logger.LogDebug("Message is too old. Ignoring...");
                return;
            }
            */

            _lastReceived = DateTime.Now;
            _serverSequenceNumber = fragmentMessage.Header.SequenceNumber;

            if (fragmentMessage.Header.IsFragment)
            {
                message = _fragmentManager.AssembleFragment(message, fragmentMessage.Header.SequenceNumber);
                if (message == null)
                {
                    Debug.WriteLine($"FragmentMessage {fragmentMessage.Header.SessionMessageType} not ready yet");
                    return;
                }
            }
            if (message is JsonMessage jm)
            {
                var json_fragment = JsonConvert.DeserializeObject<JsonMessageFragment>(jm.Json);
                if (json_fragment.datagram_id != 0)
                {
                    message = _jsonFragmentManager.HandleMessage(json_fragment);
                    if (message == null)
                    {
                        Debug.WriteLine($"JsonFragmentMessage {fragmentMessage.Header.SessionMessageType} not ready yet");
                        return;
                    }
                    message.Header = fragmentMessage.Header;
                }

            }
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs<SessionMessageBase>(message));
        }

        // TODO: When to use reject?
        private Task SendMessageAckAsync(uint[] processed = null, uint[] rejected = null,
                                                                  bool requestAck = false)
        {
            var msg = $"";
            if (processed != null)
            {
                msg += $"Acking server msg #{String.Join(",", processed)}";
            }
            else
            {
                msg = "Heatbeat Ack request";
            }

            var ackMessage = new AckMessage();
            ackMessage.Header.RequestAcknowledge = requestAck;
            ackMessage.LowWatermark = _serverSequenceNumber;
            msg += $" server seq number: {_serverSequenceNumber}";
            ackMessage.ProcessedList = new HashSet<uint>(processed ?? new uint[0]);
            ackMessage.RejectedList = new HashSet<uint>(rejected ?? new uint[0]);
            logger.LogTrace(msg);
            return SendAsync(ackMessage);
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

            message.Header.ChannelId = fragment.Header.ChannelId;
            message.Header.RequestAcknowledge = fragment.Header.RequestAcknowledge;
            message.Header.IsFragment = fragment.Header.IsFragment;
            message.Header.SessionMessageType = fragment.Header.SessionMessageType;
            message.Header.Version = fragment.Header.Version;

            logger.LogTrace($"Received fragment size: {fragment.Fragment.Length}");

            message.Deserialize(new EndianReader(fragment.Fragment));

            return message;
        }

        private Task SendFragmentAsync(SessionMessageBase message, uint sequenceNumber)
        {
            var fragment = new SessionFragmentMessage();

            fragment.Header.ChannelId = message.Header.ChannelId;
            fragment.Header.RequestAcknowledge = message.Header.RequestAcknowledge;
            fragment.Header.SessionMessageType = message.Header.SessionMessageType;
            fragment.Header.Version = message.Header.Version;

            fragment.Header.SequenceNumber = sequenceNumber;
            fragment.Header.SourceParticipantId = _participantId;

            var writer = new EndianWriter();
            message.Serialize(writer);
            fragment.Fragment = writer.ToBytes();

            logger.LogTrace($"Send fragment size: {fragment.Fragment.Length}");

            return _transport.SendAsync(fragment);
        }

        public Task SendAsync(SessionMessageBase message)
        {
            lock (_lockObject)
            {
                _sequenceNumber = _sequenceNumber + 1;
                var sequenceNumber = _sequenceNumber;

                logger.LogTrace($"Sending message #{sequenceNumber} ({message}) ...");

                if (message.Header.RequestAcknowledge)
                {
                    return Common.TaskExtensions.WithRetries(async () =>
                    {
                        var ackMessage = await WaitForMessageAsync<AckMessage>(
                            TimeSpan.FromSeconds(5),
                            async () => await SendFragmentAsync(message, sequenceNumber),
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

        public Task<SessionMessageBase> WaitForMessageAsync(TimeSpan timeout, Func<Task> startAction = null)
        {
            return this.WaitForMessageAsync<SessionMessageBase, SessionMessageBase>(timeout, startAction);
        }

        public Task<T> WaitForMessageAsync<T>(TimeSpan timeout, Func<Task> startAction = null, Func<T, bool> filter = null)
            where T : SessionMessageBase
        {
            return this.WaitForMessageAsync<T, SessionMessageBase>(timeout, startAction, filter);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try
                    {
                        SendAsync(new DisconnectMessage())
                            .Wait();
                    }
                    catch
                    {
                        // TODO: Trace
                    }

                    _cancellationTokenSource.Cancel();
                    _transport.MessageReceived -= TransportMessageReceived;
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}