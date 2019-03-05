using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using SmartGlass.Common;
using Microsoft.Extensions.Logging;

namespace SmartGlass.Channels
{
   /// <summary>
   /// Auxiliary stream client.
   /// </summary>
    public class AuxiliaryStreamClient : IDisposable
    {
        // TODO: Connection state events.
        private static readonly ILogger logger = Logging.Factory.CreateLogger<AuxiliaryStreamClient>();

        private readonly TcpClient _client;
        private readonly string _addressOrHostname;
        private readonly int _port;
        private readonly AuxiliaryStreamCryptoContext _cryptoContext;

        /// <summary>
        /// Gets the address or hostname.
        /// </summary>
        /// <value>The address or hostname.</value>
        public string AddressOrHostname => _addressOrHostname;
        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port => _port;

        /// <summary>
        /// Invoked with decrypted auxiliary stream data.
        /// </summary>
        public event EventHandler<AuxiliaryStreamDataReceivedEventArgs> DataReceived;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SmartGlass.Channels.AuxiliaryStreamClient"/> class.
        /// </summary>
        /// <param name="addressOrHostname">Address or hostname to connect to.</param>
        /// <param name="port">Port to connect to.</param>
        /// <param name="cryptoContext">Crypto context.</param>
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

        /// <summary>
        /// Reads the auxiliary stream chunks.
        /// </summary>
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

        /// <summary>
        /// Reads and decrypts auxiliary channel chunk.
        /// </summary>
        /// <returns>Decrypted chunk.</returns>
        /// <param name="stream">Stream.</param>
        private byte[] ReadAndDecryptChunk(Stream stream)
        {
            var reader = new BEReader(stream);

            // 0xde, 0xad
            reader.ReadBytes(2);

            var length = reader.ReadUInt16();

            var encryptedPayloadLength = length + Padding.CalculatePaddingSize(length, 16);

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

            var messageSignature = _cryptoContext.CalculateMessageSignature(bodyWriter.ToBytes());

            if (!signature.SequenceEqual(messageSignature))
            {
                throw new InvalidDataException("Invalid message signature.");
            }

            var decryptedPayload = _cryptoContext.Decrypt(encryptedPayloadBytes);

            return decryptedPayload.Take(length).ToArray();
        }

        /// <summary>
        /// Connects asynchronously
        /// </summary>
        /// <returns>Task.</returns>
        internal async Task ConnectAsync()
        {
            await _client.ConnectAsync(_addressOrHostname, _port);
            ReadChunks();
        }

        /// <summary>
        /// Sends an auxiliary chunk.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="bytes">Plaintext bytes.</param>
        public async Task SendAsync(byte[] bytes)
        {
            logger.LogTrace($"Sending auxiliary stream buffer: {bytes.Length} bytes");

            var writer = new BEWriter();
            writer.Write(new byte[] { 0xde, 0xad });
            writer.Write((ushort)bytes.Length);
            writer.Write(_cryptoContext.Encrypt(bytes));
            writer.Write(_cryptoContext.CalculateMessageSignature(writer.ToBytes()));

            var buffer = writer.ToBytes();

            await _client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
