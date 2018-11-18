using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SmartGlass.Common;
using SmartGlass.Connection;

namespace SmartGlass.Messaging
{
    internal class MessageTransport : IDisposable, IMessageTransport<IMessage>
    {
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
            _addressOrHostname = addressOrHostname;
            _crypto = crypto;

            _client = _addressOrHostname != null ?
                new UdpClient(_addressOrHostname, 5050) :
                new UdpClient(AddressFamily.InterNetwork);

            if (_addressOrHostname == null)
            {
                _client.Client.Bind(new IPEndPoint(IPAddress.Any, 0));
            }

            _cancellationTokenSource = _client.ConsumeReceived(receiveResult =>
            {
                var reader = new BEReader(receiveResult.Buffer);

                var messageType = (MessageType)reader.ReadUInt16();
                reader.Position = 0;

                var message = CreateFromMessageType(messageType);
                if (message == null)
                {
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

                _receiveQueue.TryAdd(message);
            });

            Task.Run(() =>
            {
                while (!_receiveQueue.IsCompleted)
                {
                    try
                    {
                        var message = _receiveQueue.Take(_cancellationTokenSource.Token);
                        MessageReceived?.Invoke(this, new MessageReceivedEventArgs<IMessage>(message));
                    }
                    catch (OperationCanceledException)
                    {
                        // pass
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(
                            $"Calling MessageReceived failed! error: {e.Message}");
                    }
                }
            });
        }

        public async Task SendAsync(IMessage message)
        {
            var cryptoMessage = message as ICryptoMessage;
            if (cryptoMessage != null)
            {
                cryptoMessage.Crypto = _crypto;
            }

            var writer = new BEWriter();
            message.Serialize(writer);
            var serialized = writer.ToBytes();

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

        public Task<IMessage> WaitForMessageAsync(TimeSpan timeout, Action startAction)
        {
            return this.WaitForMessageAsync<IMessage, IMessage>(timeout, startAction);
        }

        public Task<T> WaitForMessageAsync<T>(TimeSpan timeout, Action startAction, Func<T, bool> filter = null)
            where T : IMessage
        {
            return this.WaitForMessageAsync<T, IMessage>(timeout, startAction, filter);
        }

        public void Dispose()
        {
            _receiveQueue.CompleteAdding();
            _cancellationTokenSource.Cancel();
            _client.Dispose();
        }
    }
}
