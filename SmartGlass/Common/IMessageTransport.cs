using System;
using System.Threading.Tasks;

namespace SmartGlass.Common
{
    public interface IMessageTransport<TMessage>
    {
        event EventHandler<MessageReceivedEventArgs<TMessage>> MessageReceived;

        Task SendAsync(TMessage message);
    }
}