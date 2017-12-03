using System;
using System.Threading.Tasks;
using DarkId.SmartGlass.Messaging.Session.Messages;

namespace DarkId.SmartGlass.Channels
{
    public class TitleChannel : IDisposable
    {
        private readonly ChannelMessageTransport _transport;

        internal TitleChannel(ChannelMessageTransport transport)
        {
            _transport = transport;
        }

        public async Task<AuxiliaryStreamClient> OpenAuxiliaryStreamAsync()
        {
            var auxiliaryStreamMessage = await _transport.WaitForMessageAsync<AuxiliaryStreamMessage>(
                TimeSpan.FromSeconds(1),
                () => _transport.SendAsync(new AuxiliaryStreamMessage()).Wait(),
                m => m.ConnectionInfo != null);

            var cryptoContext = new AuxiliaryStreamCryptoContext(
                auxiliaryStreamMessage.ConnectionInfo.CryptoKey,
                auxiliaryStreamMessage.ConnectionInfo.ServerInitVector,
                auxiliaryStreamMessage.ConnectionInfo.ClientInitVector,
                auxiliaryStreamMessage.ConnectionInfo.SignHash);

            var transport = new AuxiliaryStreamClient(
                auxiliaryStreamMessage.ConnectionInfo.Endpoints[0].Host,
                int.Parse(auxiliaryStreamMessage.ConnectionInfo.Endpoints[0].Service),
                cryptoContext);

            await transport.ConnectAsync();

            return transport;
        }

        public void Dispose()
        {
            _transport.Dispose();
        }
    }
}