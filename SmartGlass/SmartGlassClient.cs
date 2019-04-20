using System;
using System.Threading.Tasks;
using SmartGlass.Common;
using SmartGlass.Messaging;
using SmartGlass.Messaging.Connection;
using SmartGlass.Messaging.Session;
using SmartGlass.Messaging.Session.Messages;
using SmartGlass.Connection;
using SmartGlass.Channels;

namespace SmartGlass
{
    public class SmartGlassClient : IDisposable
    {
        private bool _disposed = false;
        private static readonly TimeSpan connectTimeout = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan[] connectRetries = new TimeSpan[]
        {
            TimeSpan.FromMilliseconds(500),
            TimeSpan.FromMilliseconds(500),
            TimeSpan.FromMilliseconds(1000)
        };

        /// <summary>
        /// Connect to console via ip address
        /// </summary>
        /// <param name="addressOrHostname"></param>
        /// <param name="xboxLiveUserHash"></param>
        /// <param name="xboxLiveAuthorization"></param>
        /// <returns></returns>
        public static async Task<SmartGlassClient> ConnectAsync(
            string addressOrHostname, string xboxLiveUserHash = null, string xboxLiveAuthorization = null)
        {
            var device = await Device.PingAsync(addressOrHostname).ConfigureAwait(true);
            return await ConnectAsync(device, xboxLiveUserHash, xboxLiveAuthorization);
        }

        /// <summary>
        /// Connect to a discovered console
        /// </summary>
        /// <param name="device"></param>
        /// <param name="xboxLiveUserHash"></param>
        /// <param name="xboxLiveAuthorization"></param>
        /// <returns></returns>
        public static async Task<SmartGlassClient> ConnectAsync(
            Device device, string xboxLiveUserHash, string xboxLiveAuthorization)
        {
            var cryptoContext = new CryptoContext(device.Certificate);

            using (var transport = new MessageTransport(device.Address.ToString(), cryptoContext))
            {
                Guid deviceId = Guid.NewGuid();
                Func<Task> connectFunc = async () =>
                {
                    foreach (var fragment in ConnectRequestMessage
                        .GenerateConnectRequest(deviceId, cryptoContext, xboxLiveUserHash, xboxLiveAuthorization))
                    {
                        await transport.SendAsync(fragment);
                    }
                };

                var response = await Common.TaskExtensions.WithRetries(() =>
                    transport.WaitForMessageAsync<ConnectResponseMessage>(
                        connectTimeout,
                        connectFunc),
                    connectRetries);

                return new SmartGlassClient(
                    device,
                    response,
                    cryptoContext);
            }
        }

        private readonly MessageTransport _messageTransport;
        private readonly SessionMessageTransport _sessionMessageTransport;

        private uint _channelRequestId = 1;

        public InputChannel InputChannel { get; private set; }
        public InputTVRemoteChannel InputTvRemoteChannel { get; private set; }
        public MediaChannel MediaChannel { get; private set; }
        public TextChannel TextChannel { get; private set; }
        public BroadcastChannel BroadcastChannel { get; private set; }

        public event EventHandler<ConsoleStatusChangedEventArgs> ConsoleStatusChanged;

        public ConsoleStatus CurrentConsoleStatus { get; private set; }

        private SmartGlassClient(
            Device device,
            ConnectResponseMessage connectResponse,
            CryptoContext cryptoContext)
        {
            _messageTransport = new MessageTransport(device.Address.ToString(), cryptoContext);
            _sessionMessageTransport = new SessionMessageTransport(
                _messageTransport,
                new SessionInfo()
                {
                    ParticipantId = connectResponse.ParticipantId
                });

            _sessionMessageTransport.MessageReceived += (s, e) =>
            {
                var consoleStatusMessage = e.Message as ConsoleStatusMessage;
                if (consoleStatusMessage != null)
                {
                    CurrentConsoleStatus = new ConsoleStatus()
                    {
                        Configuration = consoleStatusMessage.Configuration,
                        ActiveTitles = consoleStatusMessage.ActiveTitles
                    };

                    ConsoleStatusChanged?.Invoke(this, new ConsoleStatusChangedEventArgs(
                        CurrentConsoleStatus
                    ));
                }
            };

            _sessionMessageTransport.SendAsync(new LocalJoinMessage()).Wait();
            OpenChannels().Wait();
            _sessionMessageTransport.StartHeartbeat();
        }

        private async Task OpenChannels()
        {
            InputChannel = new InputChannel(
                await StartChannelAsync(ServiceType.SystemInput));
            /*
             *  InputTvRemoteChannel fails when connecting non-authenticated
             *  (Either a bug or feature from Microsoft!)
             *  Simply disabling it for now - it serves no use anyways atm
            InputTvRemoteChannel = new InputTVRemoteChannel(
                await StartChannelAsync(ServiceType.SystemInputTVRemote));
            */
            MediaChannel = new MediaChannel(
                await StartChannelAsync(ServiceType.SystemMedia));
            TextChannel = new TextChannel(
                await StartChannelAsync(ServiceType.SystemText));
            BroadcastChannel = new BroadcastChannel(
                await StartChannelAsync(ServiceType.SystemBroadcast));
        }

        public Task LaunchTitleAsync(
            string uri,
            ActiveTitleLocation location = ActiveTitleLocation.Default)
        {
            return _sessionMessageTransport.SendAsync(new TitleLaunchMessage()
            {
                Uri = uri,
                Location = location
            });
        }

        public Task LaunchTitleByTitleIdAsync(
            uint titleId,
            ActiveTitleLocation location = ActiveTitleLocation.Default)
        {
            string uri = string.Format("ms-xbl-{0:X8}://default", titleId);
            return LaunchTitleAsync(uri, location);
        }

        public Task GameDvrRecord(int lastSeconds = 60)
        {
            return _sessionMessageTransport.SendAsync(new GameDvrRecordMessage()
            {
                StartTimeDelta = -lastSeconds,
            });
        }

        private async Task<ChannelMessageTransport> StartChannelAsync(ServiceType serviceType, uint titleId = 0)
        {
            bool timedOut = false;
            StartChannelResponseMessage response = null;

            var requestId = _channelRequestId++;

            var channelRequestMessage = new StartChannelRequestMessage()
            {
                ChannelRequestId = requestId,
                ServiceType = serviceType,
                TitleId = titleId
            };

            // TODO: Formalize timeouts for response based messages.
            try
            {
                response = await _sessionMessageTransport.WaitForMessageAsync<StartChannelResponseMessage>(
                    TimeSpan.FromSeconds(5),
                    () => _sessionMessageTransport.SendAsync(channelRequestMessage),
                    m => m.ChannelRequestId == requestId);
            }
            catch (TimeoutException)
            {
                timedOut = true;
            }

            if (timedOut || response.Result != 0)
            {
                string errorMsg = String.Format("{0} occured when opening ServiceChannel {1}.",
                    timedOut ? "Timeout" : "Rejection",
                    serviceType);

                throw new SmartGlassException(errorMsg, response.Result);
            }

            return new ChannelMessageTransport(response.ChannelId, _sessionMessageTransport);
        }

        // TODO: Show pairing state
        // TODO: Should the channel object be responsible for reestablishment when reconnection support is added?

        public async Task<TitleChannel> StartTitleChannelAsync(uint titleId)
        {
            var channel = await StartChannelAsync(ServiceType.None, titleId);

            // TODO: See if this is an aux hello message that is only sent if available.
            // Currently waiting here as a convenience to prevent opening the stream before
            // this is received.

            try
            {
                await channel.WaitForMessageAsync<AuxiliaryStreamMessage>(TimeSpan.FromSeconds(1));
            }
            catch (TimeoutException)
            {
            }

            return new TitleChannel(channel);
        }

        public async Task PowerOffAsync()
        {
            await _sessionMessageTransport.SendAsync(new PowerOffMessage());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // TODO: Close opened channels?
                    // Assuming so for the time being, but don't know how to send stop messages yet
                    InputChannel.Dispose();
                    // InputTvRemoteChannel.Dispose();

                    TextChannel.Dispose();
                    MediaChannel.Dispose();
                    BroadcastChannel.Dispose();

                    _sessionMessageTransport.Dispose();
                    _messageTransport.Dispose();
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
