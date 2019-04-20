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
        private bool _disposed = false;
        private static readonly ILogger logger = Logging.Factory.CreateLogger<JsonMessageTransport<TMessage>>();
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly IMessageTransport<SessionMessageBase> _baseTransport;

        /// <summary>
        /// Invoked when message received.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs<TMessage>> MessageReceived;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SmartGlass.Channels.JsonMessageTransport`1"/> class.
        /// </summary>
        /// <param name="baseTransport">Base transport.</param>
        /// <param name="settings">Settings.</param>
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

        public Task SendAsync(TMessage message)
        {
            return _baseTransport.SendAsync(new JsonMessage() { Json = JsonConvert.SerializeObject(message, _serializerSettings) });
        }

        public Task<TMessage> WaitForMessageAsync(TimeSpan timeout, Func<Task> startAction = null)
        {
            return this.WaitForMessageAsync<TMessage, TMessage>(timeout, startAction);
        }

        public Task<T> WaitForMessageAsync<T>(TimeSpan timeout, Func<Task> startAction = null, Func<T, bool> filter = null)
            where T : TMessage
        {
            return this.WaitForMessageAsync<T, TMessage>(timeout, startAction, filter);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _baseTransport.MessageReceived -= TransportMessageReceived;
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
