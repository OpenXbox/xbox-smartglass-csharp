using System;
using System.Threading.Tasks;
using DarkId.SmartGlass.Common;
using DarkId.SmartGlass.Messaging.Session;
using DarkId.SmartGlass.Messaging.Session.Messages;

namespace DarkId.SmartGlass.Channels
{
    internal class ChannelMessageTransport : IDisposable, IMessageTransport<SessionMessageBase>
    {
        private readonly ulong _channelId;
        private readonly SessionMessageTransport _transport;

        public event EventHandler<MessageReceivedEventArgs<SessionMessageBase>> MessageReceived;

        public ChannelMessageTransport(ulong channelId, SessionMessageTransport transport)
        {
            _channelId = channelId;
            _transport = transport;

            _transport.MessageReceived += TransportMessageReceived;
        }

        private void TransportMessageReceived(object sender, MessageReceivedEventArgs<SessionMessageBase> e)
        {
            if (e.Message.Header.ChannelId == _channelId)
            {
                MessageReceived?.Invoke(this, e);
            }
        }

        public void Dispose()
        {
            _transport.MessageReceived -= TransportMessageReceived;

            _transport.SendAsync(new StopChannelMessage()
            {
                ChannelIdToStop = _channelId
            });
        }

        public Task SendAsync(SessionMessageBase message)
        {
            message.Header.ChannelId = _channelId;
            return _transport.SendAsync(message);
        }

        public Task<SessionMessageBase> WaitForMessageAsync(TimeSpan timeout, Action startAction)
        {
            return this.WaitForMessageAsync<SessionMessageBase, SessionMessageBase>(timeout, startAction);
        }

        public Task<T> WaitForMessageAsync<T>(TimeSpan timeout, Action startAction, Func<T, bool> filter = null)
            where T : SessionMessageBase
        {
            return this.WaitForMessageAsync<T, SessionMessageBase>(timeout, startAction, filter);
        }
    }
}