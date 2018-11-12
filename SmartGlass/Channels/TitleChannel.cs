using System;
using System.Threading.Tasks;
using SmartGlass.Messaging.Session.Messages;

namespace SmartGlass.Channels
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

        public void Dispose()
        {
            _transport.Dispose();
        }
    }
}
