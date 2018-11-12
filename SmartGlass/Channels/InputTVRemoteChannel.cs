using System;
using System.Threading.Tasks;
using SmartGlass.Messaging.Session.Messages;

namespace SmartGlass.Channels
{
    public class InputTVRemoteChannel : IDisposable
    {
        private readonly ChannelMessageTransport _transport;

        internal InputTVRemoteChannel(ChannelMessageTransport transport)
        {
            _transport = transport;
        }

        public void Dispose()
        {
            _transport.Dispose();
        }
    }
}
