using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartGlass.Messaging.Session.Messages;
using SmartGlass.Common;
using SmartGlass.Messaging.Session;
using SmartGlass.Channels.Broadcast;
using SmartGlass.Channels.Broadcast.Messages;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace SmartGlass.Channels
{
    public class BroadcastChannel : IDisposable
    {
        private static readonly ILogger logger = Logging.Factory.CreateLogger<BroadcastChannel>();

        private readonly ChannelMessageTransport _baseTransport;
        private readonly JsonMessageTransport<BroadcastBaseMessage> _transport;

        private GamestreamEnabledMessage _enabledMessage;

        public bool Enabled => _enabledMessage.Enabled;
        public bool CanBeEnabled => _enabledMessage.CanBeEnabled;
        public int MajorProtocolVersion => _enabledMessage.MajorProtocolVersion;
        public int MinorProtocolVersion => _enabledMessage.MinorProtocolVersion;

        internal BroadcastChannel(ChannelMessageTransport transport)
        {
            _baseTransport = transport;
            _transport = new JsonMessageTransport<BroadcastBaseMessage>(_baseTransport, new BroadcastMessageJsonConverter());
            _transport.MessageReceived += OnMessageReceived;
        }

        internal async Task WaitForEnabledAsync()
        {
            await _transport.WaitForMessageAsync<GamestreamEnabledMessage>(TimeSpan.FromSeconds(3));
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs<BroadcastBaseMessage> e)
        {
            var enabledMessage = e.Message as GamestreamEnabledMessage;
            if (enabledMessage != null)
            {
                _enabledMessage = enabledMessage;
            }

            logger.LogTrace("Received BroadcastMsg:\r\n{0}\r\n{1}",
                e.Message.ToString(),
                JsonConvert.SerializeObject(e.Message, Formatting.Indented));
        }

        public async Task<GamestreamSession> StartGamestreamAsync()
        {
            var startMessage = new GamestreamStartMessage()
            {
                Configuration = GamestreamConfiguration.GetStandardConfig(),
                ReQueryPreviewStatus = false
            };

            var startedMessageTask = MessageExtensions.WaitForMessageAsync<GamestreamStateStartedMessage, BroadcastErrorMessage, BroadcastBaseMessage>(
                _transport,
                TimeSpan.FromSeconds(2),
                () => _transport.SendAsync(startMessage));

            var initializingMessageTask = MessageExtensions.WaitForMessageAsync
                <GamestreamStateInitializingMessage, BroadcastErrorMessage, BroadcastBaseMessage>(_transport, TimeSpan.FromSeconds(10));

            await Task.WhenAll(initializingMessageTask, startedMessageTask);

            var initializingMessage = initializingMessageTask.Result;
            var startedMessage = startedMessageTask.Result;

            if (initializingMessage.SessionId != startedMessage.SessionId)
            {
                throw new GamestreamException("Invalid session received.", GamestreamError.General);
            }

            return new GamestreamSession(initializingMessage.TcpPort, initializingMessage.UdpPort);
        }

        public void Dispose()
        {
            _transport.Dispose();
            _baseTransport.Dispose();
        }
    }
}