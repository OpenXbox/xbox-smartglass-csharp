using System;
using System.Threading.Tasks;
using SmartGlass.Messaging.Session.Messages;

namespace SmartGlass.Channels
{
    public class TextChannel : IDisposable
    {
        private readonly ChannelMessageTransport _transport;

        internal TextChannel(ChannelMessageTransport transport)
        {
            _transport = transport;
        }

        public void Dispose()
        {
            _transport.Dispose();
        }
    }
}
