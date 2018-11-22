using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SmartGlass.Common;
using SmartGlass.Nano.Channels;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano
{
    public class NanoClient : IDisposable
    {
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
        internal Provider.IProvider _provider { get; set; }

        public GamestreamConfiguration Configuration { get; private set; }
        public bool ProtocolInitialized { get; private set; }
        public bool StreamInitialized { get; private set; }
        public Guid SessionId { get; internal set; }
        public ushort ConnectionId { get; private set; }
        public ushort RemoteConnectionId => _transport.RemoteConnectionId;
        public VideoFormat[] VideoFormats => Video == null ? null : Video.AvailableFormats;
        public AudioFormat[] AudioFormats => Audio == null ? null : Audio.AvailableFormats;

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
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="tcpPort"></param>
        /// <param name="udpPort"></param>
        /// <param name="configuration"></param>
        /// <param name="sessionId"></param>
        public NanoClient(string address, int tcpPort, int udpPort,
                          GamestreamConfiguration configuration, Guid sessionId)
        {
            _transport = new NanoRdpTransport(address, tcpPort, udpPort);
            _transport.MessageReceived += MessageReceived;

            _consumers = new List<Consumer.IConsumer>();
            _provider = null;
            ProtocolInitialized = false;
            StreamInitialized = false;
            Configuration = configuration;
            SessionId = sessionId;
            ConnectionId = (ushort)new Random().Next(5000);
        }

        /// <summary>
        /// 
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
        public async Task OpenInputChannel(uint desktopWidth, uint desktopHeight)
        {
            if (!ProtocolInitialized)
            {
                throw new NanoException("Protocol is not initialized");
            }

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
        public async Task OpenChatAudioChannel(AudioFormat audioFormat)
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

            Video = new VideoChannel(_transport, video.Result.Flags);
            Audio = new AudioChannel(_transport, audio.Result.Flags);
            ChatAudio = new ChatAudioChannel(_transport, audio.Result.Flags);
            Control = new ControlChannel(_transport, audio.Result.Flags);

            // Already create Input/InputFeedback channels
            // it will get opened via "OpenInputChannel" later
            Input = new InputChannel(_transport, new byte[] { });
            InputFeedback = new InputFeedbackChannel(_transport, new byte[] { });
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
        /// Event callback for NanoTransport, just logs received packets
        /// </summary>
        /// <param name="sender">Sender of event</param>
        /// <param name="message">Received message arguments</param>
        internal void MessageReceived(object sender, MessageReceivedEventArgs<INanoPacket> message)
        {
            var packet = message.Message;
            NanoPayloadType pt = packet.Header.PayloadType;

            Debug.WriteLine($"NANO: Received {pt} on Channel <{packet.Channel}>");
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
                TimeSpan.FromSeconds(1),
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

            await TaskExtensions.WithRetries(() =>
                _transport.WaitForMessageAsync<VideoData>(
                    TimeSpan.FromSeconds(1),
                    async () => await _transport.SendAsync(packet)
                ), UdpHandshakeRetries);
        }

        public void AddConsumer(Consumer.IConsumer consumer)
        {
            Audio.FeedAudioData += consumer.ConsumeAudioData;
            Video.FeedVideoData += consumer.ConsumeVideoData;
            InputFeedback.FeedInputFeedbackFrame += consumer.ConsumeInputFeedbackFrame;
            _consumers.Add(consumer);
        }

        public bool RemoveConsumer(Consumer.IConsumer consumer)
        {
            Audio.FeedAudioData -= consumer.ConsumeAudioData;
            Video.FeedVideoData -= consumer.ConsumeVideoData;
            InputFeedback.FeedInputFeedbackFrame -= consumer.ConsumeInputFeedbackFrame;
            return _consumers.Remove(consumer);
        }

        public void Dispose()
        {
            _transport.Dispose();
        }
    }
}
