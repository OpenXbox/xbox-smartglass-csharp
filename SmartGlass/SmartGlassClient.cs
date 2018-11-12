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
        private static readonly TimeSpan connectTimeout = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan[] connectRetries = new TimeSpan[]
        {
            TimeSpan.FromMilliseconds(500),
            TimeSpan.FromMilliseconds(500),
            TimeSpan.FromMilliseconds(1500),
            TimeSpan.FromSeconds(5)
        };

        public static Task<SmartGlassClient> ConnectAsync(string addressOrHostname)
        {
            return ConnectAsync(addressOrHostname, null, null);
        }

        public static async Task<SmartGlassClient> ConnectAsync(
            string addressOrHostname, string xboxLiveUserHash, string xboxLiveAuthorization)
        {
            var device = await Device.PingAsync(addressOrHostname);
            var cryptoContext = new CryptoContext(device.Certificate);

            using (var transport = new MessageTransport(device.Address.ToString(), cryptoContext))
            {
                var deviceId = Guid.NewGuid();
                var sequenceNumber = 0u;

                var initVector = CryptoContext.GenerateRandomInitVector();

                Func<Task> connectFunc = async () =>
                {
                    var requestMessage = new ConnectRequestMessage();

                    requestMessage.InitVector = initVector;

                    requestMessage.DeviceId = deviceId;

                    requestMessage.UserHash = xboxLiveUserHash;
                    requestMessage.Authorization = xboxLiveAuthorization;

                    requestMessage.SequenceNumber = sequenceNumber;
                    requestMessage.SequenceBegin = sequenceNumber + 1;
                    requestMessage.SequenceEnd = sequenceNumber + 1;

                    sequenceNumber += 2;

                    await transport.SendAsync(requestMessage);
                };

                var response = await TaskExtensions.WithRetries(() =>
                    transport.WaitForMessageAsync<ConnectResponseMessage>(
                        connectTimeout,
                        () => connectFunc().GetAwaiter().GetResult()),
                    connectRetries);

                return new SmartGlassClient(
                    device,
                    response,
                    cryptoContext);
            }
        }

        private readonly MessageTransport _messageTransport;
        private readonly SessionMessageTransport _sessionMessageTransport;

        private readonly DisposableAsyncLazy<InputChannel> _inputChannel;
        private readonly DisposableAsyncLazy<MediaChannel> _mediaChannel;
        private readonly DisposableAsyncLazy<BroadcastChannel> _broadcastChannel;

        private uint _channelRequestId = 1;

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
                if (consoleStatusMessage != null) {
                    CurrentConsoleStatus = new ConsoleStatus() {
                        Configuration = consoleStatusMessage.Configuration,
                        ActiveTitles = consoleStatusMessage.ActiveTitles
                    };

                    ConsoleStatusChanged?.Invoke(this, new ConsoleStatusChangedEventArgs(
                        CurrentConsoleStatus
                    ));
                }
            };

            _sessionMessageTransport.SendAsync(new LocalJoinMessage());

            _inputChannel = new DisposableAsyncLazy<InputChannel>(async () =>
            {
                return new InputChannel(await StartChannelAsync(ServiceType.SystemInput));
            });

            _mediaChannel = new DisposableAsyncLazy<MediaChannel>(async () =>
            {
                return new MediaChannel(await StartChannelAsync(ServiceType.SystemMedia));
            });

            _broadcastChannel = new DisposableAsyncLazy<BroadcastChannel>(async () =>
            {
                var broadcastChannel = new BroadcastChannel(await StartChannelAsync(ServiceType.SystemBroadcast));
                await broadcastChannel.WaitForEnabledAsync();
                return broadcastChannel;
            });
        }

        public Task LaunchTitleAsync(
            uint titleId,
            string launchParams,
            ActiveTitleLocation location = ActiveTitleLocation.Default)
        {
            // TODO: Validate that Uri escape logic is correct. (Don't know of any valid existing title params.)

            return _sessionMessageTransport.SendAsync(new TitleLaunchMessage()
            {
                Uri = string.Format(
                    "ms-xbl-{0:X8}://default",
                    titleId,
                    string.IsNullOrWhiteSpace(launchParams) ?
                        string.Empty : "/" + Uri.EscapeDataString(launchParams)),
                Location = location
            });
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
            var requestId = _channelRequestId++;

            // TODO: Formalize timeouts for response based messages.
            var response = await _sessionMessageTransport.WaitForMessageAsync<StartChannelResponseMessage>(
                TimeSpan.FromSeconds(1),
                () => _sessionMessageTransport.SendAsync(new StartChannelRequestMessage()
                {
                    ChannelRequestId = requestId,
                    ServiceType = serviceType,
                    TitleId = titleId
                }).GetAwaiter().GetResult(),
                m => m.ChannelRequestId == requestId);

            if (response.Result != 0)
            {
                throw new SmartGlassException("Failed to open channel.", response.Result);
            }

            return new ChannelMessageTransport(response.ChannelId, _sessionMessageTransport);
        }

        // TODO: Show pairing state
        // TODO: Should the channel object be responsible for reestablishment when reconnection support is added?
        public Task<InputChannel> GetInputChannelAsync()
        {
            return _inputChannel.GetAsync();
        }

        public Task<MediaChannel> GetMediaChannelAsync()
        {
            return _mediaChannel.GetAsync();
        }

        public Task<BroadcastChannel> GetBroadcastChannelAsync()
        {
            return _broadcastChannel.GetAsync();
        }

        public async Task<TitleChannel> StartTitleChannelAsync(uint titleId)
        {
            var channel = await StartChannelAsync(ServiceType.None, titleId);

            // TODO: See if this is an aux hello message that is only sent if available.
            // Currently waiting here as a convenience to prevent opening the stream before
            // this is received.

            try
            {
                await channel.WaitForMessageAsync<AuxiliaryStreamMessage>(TimeSpan.FromSeconds(1), () => {});
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

        public void Dispose()
        {
            // TODO: Close opened channels?
            // Assuming so for the time being, but don't know how to send stop messages yet
            _inputChannel.Dispose();
            _mediaChannel.Dispose();

            _sessionMessageTransport.Dispose();
            _messageTransport.Dispose();
        }
    }
}
