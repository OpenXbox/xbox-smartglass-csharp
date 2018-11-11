using System;
using System.Threading.Tasks;
using DarkId.SmartGlass.Messaging.Session.Messages;

namespace DarkId.SmartGlass.Channels
{
    public class InputChannel : IDisposable
    {
        private readonly ChannelMessageTransport _transport;

        internal InputChannel(ChannelMessageTransport transport)
        {
            _transport = transport;
        }

        public async Task SendGamepadStateAsync(GamepadState state)
        {
            var message = new GamepadMessage();
            message.State = state;

            await _transport.SendAsync(message);
        }

        public void Dispose()
        {
            _transport.Dispose();
        }
    }
}