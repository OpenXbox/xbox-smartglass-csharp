using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using DarkId.SmartGlass.Common;
using Microsoft.Extensions.Logging;

namespace DarkId.SmartGlass.Channels
{
    // TODO: Connection state events.

    public class AuxiliaryStreamClient : IDisposable
    {
        private static readonly ILogger logger = Logging.Factory.CreateLogger<AuxiliaryStreamClient>();

        private readonly TcpClient _client;
        private readonly string _addressOrHostname;
        private readonly int _port;
        private readonly AuxiliaryStreamCryptoContext _cryptoContext;

        public string AddressOrHostname => _addressOrHostname;
        public int Port => _port;

        public event EventHandler<AuxiliaryStreamDataReceivedEventArgs> DataReceived;

        internal AuxiliaryStreamClient(
            string addressOrHostname,
            int port,
            AuxiliaryStreamCryptoContext cryptoContext)
        {
            _client = new TcpClient();
            _addressOrHostname = addressOrHostname;
            _port = port;

            _cryptoContext = cryptoContext;
        }

        private void ReadChunks()
        {
            Task.Run(() =>
            {
                var buffer = new byte[_client.ReceiveBufferSize];

                try
                {
                    while (_client.Connected)
                    {
                        var decrypted = ReadAndDecryptChunk(_client.GetStream());

                        logger.LogTrace(
                            $"Received auxiliary stream buffer: {decrypted.Length} bytes");

                        DataReceived?.Invoke(this, new AuxiliaryStreamDataReceivedEventArgs(decrypted));
                    }
                }
                catch (IOException)
                {
                    // TODO: Tracing
                }
                catch (SocketException)
                {
                    // TODO: Tracing
                }
                catch (ObjectDisposedException)
                {
                    // TODO: Tracing
                }
            });
        }

        private byte[] ReadAndDecryptChunk(Stream stream)
        {
            var reader = new BEReader(stream);

            // 0xde, 0xad
            reader.ReadBytes(2);

            var length = reader.ReadUInt16();

            var encryptedPayloadLength = length + BinaryExtensions.CalculatePaddingSize(length, 16);

            var encryptedPayloadBytes = new byte[encryptedPayloadLength];
            var encryptedPayloadPosition = 0;

            while (encryptedPayloadPosition < encryptedPayloadLength - 1)
            {
                var received = reader.ReadBytes(encryptedPayloadLength - encryptedPayloadPosition);
                received.CopyTo(encryptedPayloadBytes, encryptedPayloadPosition);
                encryptedPayloadPosition += received.Length;
            }

            var signature = reader.ReadBytes(32);

            var bodyWriter = new BEWriter();
            bodyWriter.Write(new byte[] { 0xde, 0xad });
            bodyWriter.Write(length);
            bodyWriter.Write(encryptedPayloadBytes);

            var messageSignature = _cryptoContext.CalculateMessageSignature(bodyWriter.ToArray());

            if (!signature.SequenceEqual(messageSignature))
            {
                throw new InvalidDataException("Invalid message signature.");
            }

            var decryptedPayload = _cryptoContext.Decrypt(encryptedPayloadBytes);

            return decryptedPayload.Take(length).ToArray();
        }

        internal async Task ConnectAsync()
        {
            await _client.ConnectAsync(_addressOrHostname, _port);
            ReadChunks();
        }

        public async Task SendAsync(byte[] bytes)
        {
            logger.LogTrace($"Sending auxiliary stream buffer: {bytes.Length} bytes");

            var writer = new BEWriter();
            writer.Write(new byte[] { 0xde, 0xad });
            writer.Write((ushort)bytes.Length);
            writer.Write(_cryptoContext.Encrypt(bytes));
            writer.Write(_cryptoContext.CalculateMessageSignature(writer.ToArray()));

            var buffer = writer.ToArray();

            await _client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}