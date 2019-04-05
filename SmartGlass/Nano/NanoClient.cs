using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartGlass.Common;
using SmartGlass.Nano.Channels;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano
{
    public class NanoClient : IDisposable
    {
        private bool _disposed = false;
        private static readonly ILogger logger = Logging.Factory.CreateLogger<NanoClient>();
        private readonly NanoRdpTransport _transport;

        private static TimeSpan[] UdpHandshakeRetries = new TimeSpan[]
        {
            TimeSpan.FromMilliseconds(100),
            TimeSpan.FromMilliseconds(200),
            TimeSpan.FromMilliseconds(400),
            TimeSpan.FromMilliseconds(800)
        };

        public AudioChannel Audio { get; private set; }
        public ChatAudioChannel ChatAudio { get; private set; }
        public ControlChannel Control { get; private set; }
        public InputChannel Input { get; private set; }
        public InputFeedbackChannel InputFeedback { get; private set; }
        public VideoChannel Video { get; private set; }

        internal List<Consumer.IConsumer> _consumers { get; set; }

        public GamestreamConfiguration Configuration { get; private set; }
        public bool ProtocolInitialized { get; private set; }
        public bool StreamInitialized { get; private set; }
        public Guid SessionId { get; internal set; }
        public ushort ConnectionId { get; private set; }
        public ushort RemoteConnectionId => _transport.RemoteConnectionId;
        public VideoFormat[] VideoFormats => Video == null ? null : Video.AvailableFormats;
        public AudioFormat[] AudioFormats => Audio == null ? null : Audio.AvailableFormats;

        public event EventHandler<AudioDataEventArgs> AudioFrameAvailable;
        public Action<AudioDataEventArgs> FireAudioFrameAvailable
            => (arg) => AudioFrameAvailable?.Invoke(this, arg);
        public event EventHandler<VideoDataEventArgs> VideoFrameAvailable;
        public Action<VideoDataEventArgs> FireVideoFrameAvailable
            => (arg) => VideoFrameAvailable?.Invoke(this, arg);
        public event EventHandler<InputFrameEventArgs> InputFeedbackFrameAvailable;
        public Action<InputFrameEventArgs> FireInputFeedbackFrameAvailable
            => (arg) => InputFeedbackFrameAvailable?.Invoke(this, arg);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="session"></param>
        public NanoClient(string address, GamestreamSession session)
            : this(address, session.TcpPort, session.UdpPort, session.Config, session.SessionId)
        {
        }

        /// <summary>
        /// Initialize an instance of NanoClient
        /// </summary>
        /// <param name="address">Console IP address string</param>
        /// <param name="tcpPort">Nano TCP port</param>
        /// <param name="udpPort">Nano UDP port</param>
        /// <param name="configuration">GamestreamConfiguration sent via BroadcastChannel *unused atm*</param>
        /// <param name="sessionId">Session ID received on BroadcastChannel *unused atm*</param>
        public NanoClient(string address, int tcpPort, int udpPort,
                          GamestreamConfiguration configuration, Guid sessionId)
        {
            _transport = new NanoRdpTransport(address, tcpPort, udpPort);

            _consumers = new List<Consumer.IConsumer>();
            ProtocolInitialized = false;
            StreamInitialized = false;
            Configuration = configuration;
            SessionId = sessionId;
            ConnectionId = (ushort)new Random().Next(5000);
        }

        /// <summary>
        /// Initialize nano protocol
        /// </summary>
        /// <returns></returns>
        public async Task InitializeProtocolAsync()
        {
            if (ProtocolInitialized)
            {
                throw new NanoException("Protocol is already initialized");
            }

            Task awaitChannels = WaitForChannelsAsync();

            ControlHandshake response = await SendControlHandshakeAsync(ConnectionId);
            if (response.Type != ControlHandshakeType.ACK)
            {
                throw new NotSupportedException(
                    $"Invalid ControlHandshake type received {response.Type}");
            }

            await awaitChannels;
            await OpenChannelsAsync();
            ProtocolInitialized = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="audioFormat"></param>
        /// <param name="videoFormat"></param>
        /// <returns></returns>
        public async Task InitializeStreamAsync(AudioFormat audioFormat, VideoFormat videoFormat)
        {
            if (!ProtocolInitialized)
            {
                throw new NanoException("Protocol is not initialized");
            }
            else if (StreamInitialized)
            {
                throw new NanoException("Stream is already initialized");
            }

            await Audio.SendClientHandshakeAsync(audioFormat);
            await Video.SendClientHandshakeAsync(videoFormat);
            StreamInitialized = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task OpenInputChannelAsync(uint desktopWidth, uint desktopHeight)
        {
            if (!ProtocolInitialized)
            {
                throw new NanoException("Protocol is not initialized");
            }

            // We have to generate ChannelOpenData to send to the console
            var inputChannelOpenData = new ChannelOpen(new byte[0]);

            Input = new InputChannel(_transport, inputChannelOpenData);
            InputFeedback = new InputFeedbackChannel(_transport, inputChannelOpenData,
                FireInputFeedbackFrameAvailable);

            // Send ControllerEvent.Added
            await _transport.WaitForMessageAsync<ChannelCreate>(
                TimeSpan.FromSeconds(3),
                async () => await Control.SendControllerEventAsync(
                    ControllerEventType.Added, 0),
                p => p.Channel == NanoChannel.Input);

            await Task.WhenAll(
                Input.OpenAsync(),
                InputFeedback.OpenAsync(desktopWidth, desktopHeight)
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task OpenChatAudioChannelAsync(AudioFormat audioFormat)
        {
            if (!ProtocolInitialized)
            {
                throw new NanoException("Protocol is not initialized");
            }

            await ChatAudio.OpenAsync(audioFormat);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task StartStreamAsync()
        {
            if (!StreamInitialized)
            {
                throw new NanoException("Stream is not initialized");
            }
            Task handshakeTask = SendUdpHandshakeAsync();

            await Video.StartStreamAsync();
            await Audio.StartStreamAsync();

            await handshakeTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task WaitForChannelsAsync()
        {
            Task<ChannelOpen> video = WaitForChannelOpenAsync(NanoChannel.Video);
            Task<ChannelOpen> audio = WaitForChannelOpenAsync(NanoChannel.Audio);
            Task<ChannelOpen> chatAudio = WaitForChannelOpenAsync(NanoChannel.ChatAudio);
            Task<ChannelOpen> control = WaitForChannelOpenAsync(NanoChannel.Control);

            await Task.WhenAll(video, audio, chatAudio, control);

            Video = new VideoChannel(_transport, video.Result, FireVideoFrameAvailable);
            Audio = new AudioChannel(_transport, audio.Result, FireAudioFrameAvailable);
            ChatAudio = new ChatAudioChannel(_transport, chatAudio.Result);
            Control = new ControlChannel(_transport, control.Result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task OpenChannelsAsync()
        {
            Task video = Video.OpenAsync();
            Task audio = Audio.OpenAsync();
            Task control = Control.OpenAsync();

            await Task.WhenAll(video, audio, control);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TChannel"></typeparam>
        /// <returns></returns>
        private async Task<ChannelOpen> WaitForChannelOpenAsync(NanoChannel channel)
        {
            return await _transport.WaitForMessageAsync<ChannelOpen>(
                TimeSpan.FromSeconds(3),
                startAction: null,
                filter: open => open.Channel == channel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private async Task<ControlHandshake> SendControlHandshakeAsync(ushort connectionId,
            ControlHandshakeType type = ControlHandshakeType.SYN)
        {
            var packet = new Packets.ControlHandshake(type, connectionId)
            {
                Channel = NanoChannel.TcpBase
            };

            return await _transport.WaitForMessageAsync<ControlHandshake>(
                TimeSpan.FromSeconds(5),
                async () => await _transport.SendAsync(packet));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private async Task SendUdpHandshakeAsync(
            ControlHandshakeType type = ControlHandshakeType.ACK)
        {
            var packet = new Packets.UdpHandshake(type)
            {
                Channel = NanoChannel.TcpBase
            };

            // FIXME: Wait for first VideoPacket
            // Currently WaitForMessage is broken and blocks VideoData
            // populating to MessageReceived events
            await _transport.SendAsync(packet);
        }

        public void AddConsumer(Consumer.IConsumer consumer)
        {
            AudioFrameAvailable += consumer.ConsumeAudioData;
            VideoFrameAvailable += consumer.ConsumeVideoData;
            InputFeedbackFrameAvailable += consumer.ConsumeInputFeedbackFrame;
            _consumers.Add(consumer);
        }

        public bool RemoveConsumer(Consumer.IConsumer consumer)
        {
            AudioFrameAvailable -= consumer.ConsumeAudioData;
            VideoFrameAvailable -= consumer.ConsumeVideoData;
            InputFeedbackFrameAvailable -= consumer.ConsumeInputFeedbackFrame;
            return _consumers.Remove(consumer);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transport.Dispose();
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
