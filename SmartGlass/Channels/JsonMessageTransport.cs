using System;
using System.Threading.Tasks;
using SmartGlass.Common;
using SmartGlass.Json;
using SmartGlass.Messaging.Session;
using SmartGlass.Messaging.Session.Messages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SmartGlass.Channels
{
    class JsonMessageTransport<TMessage> : IDisposable, IMessageTransport<TMessage>
    {
        private static readonly ILogger logger = Logging.Factory.CreateLogger<JsonMessageTransport<TMessage>>();
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IMessageTransport<SessionMessageBase> _baseTransport;

        public event EventHandler<MessageReceivedEventArgs<TMessage>> MessageReceived;

        public JsonMessageTransport(IMessageTransport<SessionMessageBase> baseTransport, JsonSerializerSettings settings)
        {
            _serializerSettings = settings;

            _baseTransport = baseTransport;
            _baseTransport.MessageReceived += TransportMessageReceived;
        }

        private void TransportMessageReceived(object sender, MessageReceivedEventArgs<SessionMessageBase> e)
        {
            if (e.Message is JsonMessage)
            {
                MessageReceived?.Invoke(this,
                    new MessageReceivedEventArgs<TMessage>(
                        JsonConvert.DeserializeObject<TMessage>(((JsonMessage)e.Message).Json, _serializerSettings)));
            }
        }

        public void Dispose()
        {
            _baseTransport.MessageReceived -= TransportMessageReceived;
        }

        public Task SendAsync(TMessage message)
        {
            return _baseTransport.SendAsync(new JsonMessage() { Json = JsonConvert.SerializeObject(message, _serializerSettings) });
        }

        public Task<TMessage> WaitForMessageAsync(TimeSpan timeout, Action startAction = null)
        {
            return this.WaitForMessageAsync<TMessage, TMessage>(timeout, startAction);
        }

        public Task<T> WaitForMessageAsync<T>(TimeSpan timeout, Action startAction = null, Func<T, bool> filter = null)
            where T : TMessage
        {
            return this.WaitForMessageAsync<T, TMessage>(timeout, startAction, filter);
        }
    }
}
