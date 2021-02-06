using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartGlass.Common;
using SmartGlass.Connection;

namespace SmartGlass.Messaging
{
    internal class MessageTransport : IDisposable, IMessageTransport<IMessage>
    {
        private static readonly ILogger logger = Logging.Factory.CreateLogger<MessageTransport>();

        private bool _disposed = false;
        private static IPAddress MULTICAST_ADDR = IPAddress.Parse("239.255.255.250");
        private static IMessage CreateFromMessageType(MessageType messageType)
        {
            var type = MessageTypeAttribute.GetTypeForMessageType(messageType);
            if (type == null)
            {
                return null;
            }

            return (IMessage)Activator.CreateInstance(type);
        }

        private readonly UdpClient _client;

        private readonly CancellationTokenSource _cancellationTokenSource;

        private readonly BlockingCollection<IMessage> _receiveQueue = new BlockingCollection<IMessage>();

        private readonly string _addressOrHostname;
        private readonly CryptoContext _crypto;

        public event EventHandler<MessageReceivedEventArgs<IMessage>> MessageReceived;

        public MessageTransport(string addressOrHostname = null)
            : this(addressOrHostname, null)
        {
        }

        public MessageTransport(string addressOrHostname, CryptoContext crypto)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _addressOrHostname = addressOrHostname;
            _crypto = crypto;

            _client = _addressOrHostname != null ?
                new UdpClient(_addressOrHostname, 5050) :
                new UdpClient(AddressFamily.InterNetwork);

            if (_addressOrHostname == null)
            {
                _client.Client.Bind(new IPEndPoint(GlobalConfiguration.BindAddress, 0));
            }

            _client.ConsumeReceived(receiveResult =>
            {
                logger.LogTrace($"Full receive message size: {receiveResult.Buffer.Length}");
                var reader = new EndianReader(receiveResult.Buffer);

                var messageType = (MessageType)reader.ReadUInt16BE();
                reader.Position = 0;

                var message = CreateFromMessageType(messageType);
                if (message == null)
                {
                    logger.LogTrace($"Failed to read message of type: {messageType}");
                    // TODO: Tracing for this.
                    return;
                }

                var cryptoMessage = message as ICryptoMessage;
                if (cryptoMessage != null)
                {
                    cryptoMessage.Crypto = _crypto;
                }

                message.Origin = receiveResult.RemoteEndPoint;
                message.ClientReceivedTimestamp = DateTime.Now;
                message.Deserialize(reader);

                logger.LogTrace($"Adding message of type {message.Header.Type} to receive queue.");

                _receiveQueue.TryAdd(message);
            }, _cancellationTokenSource.Token);

            Task.Run(() =>
            {
                while (!_receiveQueue.IsCompleted)
                {
                    try
                    {
                        var message = _receiveQueue.Take(_cancellationTokenSource.Token);
                        logger.LogTrace($"Taking message of type {message.Header.Type} from receive queue.");

                        Task.Run(() => MessageReceived?.Invoke(this, new MessageReceivedEventArgs<IMessage>(message)));
                    }
                    catch (OperationCanceledException)
                    {
                        // pass
                    }
                    catch (Exception e)
                    {
                        logger.LogError(
                            $"Calling MessageReceived failed! error: {e.Message}");
                    }
                }
            }, _cancellationTokenSource.Token);
        }

        /// <summary>
        /// Sends a packet
        /// </summary>
        /// <returns>The send task.</returns>
        /// <param name="message">SmartGlass SimpleMessage to sent</param>
        public async Task SendAsync(IMessage message)
        {
            var cryptoMessage = message as ICryptoMessage;
            if (cryptoMessage != null)
            {
                cryptoMessage.Crypto = _crypto;
            }

            var writer = new EndianWriter();
            message.Serialize(writer);
            var serialized = writer.ToBytes();

            logger.LogTrace($"Full send message size: {serialized.Length}");

            if (_addressOrHostname == null)
            {
                await _client.SendAsync(serialized, serialized.Length, new IPEndPoint(IPAddress.Broadcast, 5050));
                await _client.SendAsync(serialized, serialized.Length, new IPEndPoint(MULTICAST_ADDR, 5050));
            }
            else
            {
                await _client.SendAsync(serialized, serialized.Length);
            }
        }

        public Task<IMessage> WaitForMessageAsync(TimeSpan timeout, Func<Task> startAction = null)
        {
            return this.WaitForMessageAsync<IMessage, IMessage>(timeout, startAction);
        }

        public Task<T> WaitForMessageAsync<T>(TimeSpan timeout, Func<Task> startAction = null, Func<T, bool> filter = null)
            where T : IMessage
        {
            return this.WaitForMessageAsync<T, IMessage>(timeout, startAction, filter);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _receiveQueue.CompleteAdding();
                    _cancellationTokenSource.Cancel();
                    _client.Dispose();
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
