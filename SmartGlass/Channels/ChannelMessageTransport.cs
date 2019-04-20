using System;
using System.Threading.Tasks;
using SmartGlass.Common;
using SmartGlass.Messaging.Session;
using SmartGlass.Messaging.Session.Messages;

namespace SmartGlass.Channels
{
    /// <summary>
    /// ChannelMessageTransport is the base class for specific channel
    /// implementations (e.g. BroadcastChannel, MediaChannel etc.).
    /// </summary>
    /// <typeparam name="SessionMessageBase"></typeparam>
    internal class ChannelMessageTransport : IDisposable, IMessageTransport<SessionMessageBase>
    {
        private bool _disposed = false;
        private readonly ulong _channelId;
        private readonly SessionMessageTransport _transport;

        public event EventHandler<MessageReceivedEventArgs<SessionMessageBase>> MessageReceived;

        /// <summary>
        /// Initialize an instance of ChannelMessageTransport.
        /// </summary>
        /// <param name="channelId">Channel Id negotiated for this channel</param>
        /// <param name="transport">Instance of SessionMessageTransport</param>
        public ChannelMessageTransport(ulong channelId, SessionMessageTransport transport)
        {
            _channelId = channelId;
            _transport = transport;

            _transport.MessageReceived += TransportMessageReceived;
        }

        /// <summary>
        /// TransportMessageReceived takes a message event and passes
        /// it along to the MessageReceived EventHandler if the configured
        /// Channel Id matches.
        /// </summary>
        /// <param name="sender">Origin sender</param>
        /// <param name="e">EventArgs containing the message</param>
        private void TransportMessageReceived(object sender, MessageReceivedEventArgs<SessionMessageBase> e)
        {
            if (e.Message.Header.ChannelId == _channelId)
            {
                MessageReceived?.Invoke(this, e);
            }
        }

        /// <summary>
        /// Asynchronously send a generic session message.
        /// </summary>
        /// <param name="message">Session message</param>
        /// <returns>Task</returns>
        public Task SendAsync(SessionMessageBase message)
        {
            message.Header.ChannelId = _channelId;
            return _transport.SendAsync(message);
        }

        /// <summary>
        /// Wait for a message within a given timeout.
        /// </summary>
        /// <param name="timeout">Maximal time to wait</param>
        /// <param name="startAction">Action to take before waiting happens</param>
        /// <returns>Task returning the awaited message</returns>
        /// <exception cref="System.TimeoutException">
        /// When message is not received in time.
        /// </exception>
        public Task<SessionMessageBase> WaitForMessageAsync(TimeSpan timeout, Func<Task> startAction = null)
        {
            return this.WaitForMessageAsync<SessionMessageBase, SessionMessageBase>(timeout, startAction);
        }

        /// <summary>
        /// Wait for a specific message with a given timeout.
        /// It is possible to apply a filter to the incoming messages.
        /// </summary>
        /// <param name="timeout">Maximum time to wait for the message</param>
        /// <param name="startAction">Start action</param>
        /// <param name="filter">Optional filter for message to get returned</param>
        /// <typeparam name="T">Message type to await</typeparam>
        /// <returns>A task returning the awaited message</returns>
        public Task<T> WaitForMessageAsync<T>(TimeSpan timeout, Func<Task> startAction = null, Func<T, bool> filter = null)
            where T : SessionMessageBase
        {
            return this.WaitForMessageAsync<T, SessionMessageBase>(timeout, startAction, filter);
        }

        /// <summary>
        /// Removes the MessageReceived callback and
        /// attempts to close the channel with the configured
        /// channel Id.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transport.MessageReceived -= TransportMessageReceived;

                    _transport.SendAsync(new StopChannelMessage()
                    {
                        ChannelIdToStop = _channelId
                    });
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