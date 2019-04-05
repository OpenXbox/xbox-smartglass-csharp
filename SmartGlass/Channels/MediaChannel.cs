using System;
using System.Threading.Tasks;
using SmartGlass.Messaging.Session.Messages;

namespace SmartGlass.Channels
{
    /// <summary>
    /// Media channel.
    /// Handles controlling audio / video playback on the console.
    /// </summary>
    public class MediaChannel : IDisposable
    {
        private bool _disposed = false;
        private readonly ChannelMessageTransport _transport;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SmartGlass.Channels.MediaChannel"/> class.
        /// </summary>
        /// <param name="transport">Base transport.</param>
        internal MediaChannel(ChannelMessageTransport transport)
        {
            _transport = transport;
        }

        /// <summary>
        /// Sends a media command message.
        /// </summary>
        /// <returns>Send message task.</returns>
        /// <param name="state">State message to send.</param>
        public async Task SendMediaCommandStateAsync(MediaCommandState state)
        {
            var message = new MediaCommandMessage();
            message.State = state;

            await _transport.SendAsync(message);
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
