using System;

namespace SmartGlass.Common
{
    /// <summary>
    /// Message received event arguments.
    /// </summary>
    public class MessageReceivedEventArgs<TMessage> : EventArgs
    {
        private readonly TMessage _message;
        /// <summary>
        /// Received message.
        /// </summary>
        /// <value>The message.</value>
        public TMessage Message => _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SmartGlass.Common.MessageReceivedEventArgs`1"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public MessageReceivedEventArgs(TMessage message)
        {
            _message = message;
        }
    }
}
