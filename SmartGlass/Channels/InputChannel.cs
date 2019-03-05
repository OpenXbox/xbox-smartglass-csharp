using System;
using System.Threading.Tasks;
using SmartGlass.Messaging.Session.Messages;

namespace SmartGlass.Channels
{
    /// <summary>
    /// Input channel.
    /// Handles touch / gamepad / sensor input.
    /// </summary>
    public class InputChannel : IDisposable
    {
        private readonly ChannelMessageTransport _transport;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SmartGlass.Channels.InputChannel"/> class.
        /// </summary>
        /// <param name="transport">Base transport.</param>
        internal InputChannel(ChannelMessageTransport transport)
        {
            _transport = transport;
        }

        /// <summary>
        /// Sends a gamepad state message.
        /// </summary>
        /// <returns>Message send task.</returns>
        /// <param name="state">Gamepad state.</param>
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
