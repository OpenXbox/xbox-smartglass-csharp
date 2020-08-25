using System;
using System.Threading.Tasks;
using SmartGlass.Common;
using SmartGlass.Channels.Broadcast;
using SmartGlass.Channels.Broadcast.Messages;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using SmartGlass.Json;

namespace SmartGlass.Channels
{
    /// <summary>
    /// BroadcastChannel handles the initial communication with the
    /// console to initialize a GameStreaming session.
    /// <para>
    /// You need to provide a GamestreamConfiguration to make the
    /// console start gamestreaming. It is possible that the
    /// console you connect to has gamestreaming functionality disabled,
    /// check <see cref="GamestreamEnabledReceived"/> and <see cref="Enabled"/>.
    /// </para>
    /// </summary>
    public class BroadcastChannel : IDisposable
    {
        private bool _disposed = false;
        private static readonly ILogger logger = Logging.Factory.CreateLogger<BroadcastChannel>();

        private readonly ChannelMessageTransport _baseTransport;
        private readonly JsonMessageTransport<BroadcastBaseMessage> _transport;

        private GamestreamEnabledMessage _enabledMessage;

        /// <summary>
        /// A value indicating wether the GamestreamEnabled message
        /// has been received.
        /// </summary>
        public bool GamestreamEnabledReceived =>
            _enabledMessage != null;
        
        /// <summary>
        /// A value indicating wether the gamestreaming ability
        /// is enabled on the console.
        /// <seealso cref="BroadcastChannel.GamestreamEnabledReceived"/>
        /// </summary>
        public bool Enabled =>
            _enabledMessage != null && _enabledMessage.Enabled;

        /// <summary>
        /// A value indicating wether the gamestreaming ability
        /// can be enabled on the console.
        /// <seealso cref="BroadcastChannel.GamestreamEnabledReceived"/>
        /// </summary>
        public bool CanBeEnabled =>
            _enabledMessage != null && _enabledMessage.CanBeEnabled;
        
        /// <summary>
        /// Major gamestreaming protocol version the console is using.
        /// <seealso cref="BroadcastChannel.GamestreamEnabledReceived"/>
        /// </summary>
        public int MajorProtocolVersion =>
            _enabledMessage != null ? _enabledMessage.MajorProtocolVersion : 0;
        
        /// <summary>
        /// Minor gamestreaming protocol version the console is using.
        /// <seealso cref="BroadcastChannel.GamestreamEnabledReceived"/>
        /// </summary>
        public int MinorProtocolVersion =>
            _enabledMessage != null ? _enabledMessage.MinorProtocolVersion : 0;

        /// <summary>
        /// Initialize class of BroadcastChannel.
        /// Internally done by <see cref="SmartGlass.SmartGlassClient"/>
        /// </summary>
        /// <param name="transport">ChannelMessage transport</param>
        internal BroadcastChannel(ChannelMessageTransport transport)
        {
            _baseTransport = transport;
            _transport = new JsonMessageTransport<BroadcastBaseMessage>(_baseTransport, ChannelJsonSerializerOptions.GetBroadcastOptions());
            _transport.MessageReceived += OnMessageReceived;
        }

        /// <summary>
        /// Currently unused
        /// </summary>
        /// <returns>Task</returns>
        public Task WaitForEnabledAsync(TimeSpan timeout)
        {
            return Task.Run(() =>
            {
                var start = DateTime.Now;
                while(DateTime.Now - start < timeout)
                {
                    if (GamestreamEnabledReceived)
                        return;
                }
                throw new TimeoutException();
            });
        }

        /// <summary>
        /// OnMessageReceived is called internally when a new broadcast
        /// message comes in.
        /// </summary>
        /// <param name="sender">Origin sender</param>
        /// <param name="e">EventArgs containing the broadcast message</param>
        private void OnMessageReceived(object sender, MessageReceivedEventArgs<BroadcastBaseMessage> e)
        {
            if (e.Message is GamestreamEnabledMessage enabledMessage)
            {
                _enabledMessage = enabledMessage;
            }

            logger.LogTrace("Received BroadcastMsg:\r\n{0}\r\n{1}",
                e.Message.ToString(),
                JsonSerializer.Serialize(e.Message, new JsonSerializerOptions() { WriteIndented = true }));
        }

        /// <summary>
        /// Initiate a Gamestream session with the host.
        /// </summary>
        /// <param name="configuration">Desired gamestream configuration</param>
        /// <returns>Task returning the GamestreamSession object</returns>
        /// <exception cref="System.TimeoutException">
        /// When console does not respond in time
        /// </exception>
        /// <exception cref="SmartGlass.Channels.Broadcast.GamestreamException">
        /// On unexpected GamestreamSession
        /// </exception>
        public async Task<GamestreamSession> StartGamestreamAsync(GamestreamConfiguration configuration)
        {
            var startMessage = new GamestreamStartMessage()
            {
                Configuration = configuration,
                ReQueryPreviewStatus = false
            };

            var startedMessageTask = MessageExtensions.WaitForMessageAsync<GamestreamStateStartedMessage, BroadcastErrorMessage, BroadcastBaseMessage>(
                _transport,
                TimeSpan.FromSeconds(10),
                async () => await _transport.SendAsync(startMessage));

            var initializingMessageTask = MessageExtensions.WaitForMessageAsync
                <GamestreamStateInitializingMessage, BroadcastErrorMessage, BroadcastBaseMessage>(_transport, TimeSpan.FromSeconds(10));

            await Task.WhenAll(initializingMessageTask, startedMessageTask);

            var initializingMessage = initializingMessageTask.Result;
            var startedMessage = startedMessageTask.Result;

            if (initializingMessage.SessionId != startedMessage.SessionId)
            {
                throw new GamestreamException("Invalid session received.", GamestreamError.General);
            }

            return new GamestreamSession(initializingMessage.TcpPort, initializingMessage.UdpPort, configuration, initializingMessage.SessionId);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transport.Dispose();
                    _baseTransport.Dispose();
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
