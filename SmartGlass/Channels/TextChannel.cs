using System;
using System.Threading.Tasks;
using SmartGlass.Messaging.Session.Messages;

namespace SmartGlass.Channels
{
    /// <summary>
    /// Text channel.
    /// Handles entering text on the console, instead of using the
    /// on-screen-keyboard.
    /// </summary>
    public class TextChannel : IDisposable
    {
        private readonly ChannelMessageTransport _transport;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SmartGlass.Channels.TextChannel"/> class.
        /// </summary>
        /// <param name="transport">Base transport.</param>
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
