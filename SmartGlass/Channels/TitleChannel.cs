using System;
using System.Threading.Tasks;
using SmartGlass.Messaging.Session.Messages;

namespace SmartGlass.Channels
{
    /// <summary>
    /// Title channel.
    /// Supported titles: Fallout 4 (Are there more supported games ?!)
    /// </summary>
    public class TitleChannel : IDisposable
    {
        private bool _disposed = false;
        private readonly ChannelMessageTransport _transport;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SmartGlass.Channels.TitleChannel"/> class.
        /// </summary>
        /// <param name="transport">Transport.</param>
        internal TitleChannel(ChannelMessageTransport transport)
        {
            _transport = transport;
        }

        public async Task<AuxiliaryStreamClient> OpenAuxiliaryStreamAsync()
        {
            var auxiliaryStreamMessage = await _transport.WaitForMessageAsync<AuxiliaryStreamMessage>(
                TimeSpan.FromSeconds(1),
                () => _transport.SendAsync(new AuxiliaryStreamMessage()).GetAwaiter().GetResult(),
                m => m.ConnectionInfo != null);

            var cryptoContext = new AuxiliaryStreamCryptoContext(
                auxiliaryStreamMessage.ConnectionInfo.CryptoKey,
                auxiliaryStreamMessage.ConnectionInfo.ServerInitVector,
                auxiliaryStreamMessage.ConnectionInfo.ClientInitVector,
                auxiliaryStreamMessage.ConnectionInfo.SignHash);

            var client = new AuxiliaryStreamClient(
                auxiliaryStreamMessage.ConnectionInfo.Endpoints[0].Host,
                int.Parse(auxiliaryStreamMessage.ConnectionInfo.Endpoints[0].Service),
                cryptoContext);

            await client.ConnectAsync();

            return client;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transport.Dispose();
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
