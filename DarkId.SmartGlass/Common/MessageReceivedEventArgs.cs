using System;

namespace DarkId.SmartGlass.Common
{
    internal class MessageReceivedEventArgs<TMessage> : EventArgs
    {
        private readonly TMessage _message;
        public TMessage Message => _message;

        public MessageReceivedEventArgs(TMessage message)
        {
            _message = message;
        }
    }
}