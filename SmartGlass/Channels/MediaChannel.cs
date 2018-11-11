using System;
using System.Threading.Tasks;
using SmartGlass.Messaging.Session.Messages;

namespace SmartGlass.Channels
{
    public class MediaChannel : IDisposable
    {
        private readonly ChannelMessageTransport _transport;

        internal MediaChannel(ChannelMessageTransport transport)
        {
            _transport = transport;
        }

        public async Task SendMediaCommandStateAsync(MediaCommandState state)
        {
            var message = new MediaCommandMessage();
            message.State = state;

            await _transport.SendAsync(message);
        }

        public void Dispose()
        {
            _transport.Dispose();
        }
    }
}
