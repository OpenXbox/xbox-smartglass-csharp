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

        public Guid SessionId { get; internal set; }
        public ushort ConnectionId { get; private set; }
        public ushort RemoteConnectionId { get; private set; }

        public NanoClient(string address, GamestreamSession session)
            : this(address, session.TcpPort, session.UdpPort, session.Config, session.SessionId)
        {
        }

        public NanoClient(string address, int tcpPort, int udpPort,
                          GamestreamConfiguration configuration, Guid sessionId)
        {
            _transport = new NanoRdpTransport(address, tcpPort, udpPort);
            _transport.MessageReceived += MessageReceived;
            _consumers = new List<Consumer.IConsumer>();
            _provider = null;
            SessionId = sessionId;
            ConnectionId = (ushort)new Random().Next(5000);
        }

        private IStreamingChannel GetChannelById(NanoChannel id)
        {
            switch (id)
            {
                case NanoChannel.Audio: return Audio;
                case NanoChannel.ChatAudio: return ChatAudio;
                case NanoChannel.Input: return Input;
                case NanoChannel.InputFeedback: return InputFeedback;
                case NanoChannel.Video: return Video;
                case NanoChannel.Control: return Control;
                default:
                    throw new NotSupportedException($"Unsupported NanoChannel: {id}");
            }
        }

        public async Task Initialize()
        {
            Task awaitChannels = WaitForChannels();

            ControlHandshake response = await SendControlHandshakeAsync(ConnectionId);
            if (response.Type != ControlHandshakeType.ACK)
            {
                throw new NotSupportedException(
                    $"Invalid ControlHandshake type received {response.Type}");
            }
            RemoteConnectionId = response.ConnectionId;

            await awaitChannels;
            await OpenChannels();
        }

        public async Task StartStream()
        {
            Task handshakeTask = SendUdpHandshakeAsync();

            Video.StartStream();
            Audio.StartStream();

            await handshakeTask;
        }

        private async Task WaitForChannels()
        {
            Task<VideoChannel> video = WaitForChannelOpen<VideoChannel>();
            Task<AudioChannel> audio = WaitForChannelOpen<AudioChannel>();
            Task<ChatAudioChannel> chatAudio = WaitForChannelOpen<ChatAudioChannel>();
            Task<ControlChannel> control = WaitForChannelOpen<ControlChannel>();

            await Task.WhenAll(video, audio, chatAudio, control);

            Video = video.Result;
            Audio = audio.Result;
            ChatAudio = chatAudio.Result;
            Control = control.Result;
        }

        private async Task OpenChannels()
        {
            Task video = Video.OpenAsync();
            Task audio = Audio.OpenAsync();
            Task chatAudio = ChatAudio.OpenAsync();
            Task control = Control.OpenAsync();

            await Task.WhenAll(video, audio, chatAudio, control);
        }

        private async Task<TChannel> WaitForChannelOpen<TChannel>()
            where TChannel : StreamingChannel, new()
        {
            TChannel channel = new TChannel();
            var openPacket = await WaitForMessageAsync<ChannelOpen>(
                TimeSpan.FromSeconds(3),
                startAction: null,
                filter: open => open.Channel == channel.Channel);

            channel.RegisterOpen(this, openPacket.Flags);
            return channel;
        }

        internal void MessageReceived(object sender, MessageReceivedEventArgs<INanoPacket> message)
        {
            var packet = message.Message;
            NanoPayloadType pt = packet.Header.PayloadType;

            Debug.WriteLine($"NANO: Received {pt} on Channel <{packet.Channel}>");

            if (packet as IStreamerMessage != null)
                GetChannelById(packet.Channel).OnPacket((IStreamerMessage)packet);
        }

        private async Task<ControlHandshake> SendControlHandshakeAsync(ushort connectionId,
            ControlHandshakeType type = ControlHandshakeType.SYN)
        {
            var packet = new Packets.ControlHandshake(type, connectionId);

            return await WaitForMessageAsync<ControlHandshake>(
                TimeSpan.FromSeconds(1),
                async () => await SendOnControlSocketAsync(packet));
        }

        private async Task SendUdpHandshakeAsync(
            ControlHandshakeType type = ControlHandshakeType.ACK)
        {
            var packet = new Packets.UdpHandshake(type);

            await TaskExtensions.WithRetries(() =>
                WaitForMessageAsync<VideoData>(
                    TimeSpan.FromSeconds(1),
                    async () => await SendOnStreamingSocketAsync(packet)
                ), UdpHandshakeRetries);
        }

        internal Task<INanoPacket> WaitForMessageAsync(TimeSpan timeout, Action startAction)
        {
            return _transport.WaitForMessageAsync<INanoPacket, INanoPacket>(timeout, startAction);
        }

        internal Task<T> WaitForMessageAsync<T>(TimeSpan timeout, Action startAction, Func<T, bool> filter = null)
            where T : INanoPacket
        {
            return _transport.WaitForMessageAsync<T, INanoPacket>(timeout, startAction, filter);
        }

        internal async Task SendOnStreamingSocketAsync(INanoPacket packet)
        {
            packet.Header.ConnectionId = RemoteConnectionId;
            await _transport.SendAsyncStreaming(packet);
        }

        internal async Task SendOnControlSocketAsync(INanoPacket packet)
        {
            await _transport.SendAsyncControl(packet);
        }

        public void AddConsumer(Consumer.IConsumer consumer)
        {
            Audio.FeedAudioFormat += consumer.ConsumeAudioFormat;
            Audio.FeedAudioData += consumer.ConsumeAudioData;
            Video.FeedVideoFormat += consumer.ConsumeVideoFormat;
            Video.FeedVideoData += consumer.ConsumeVideoData;

            InputFeedback.FeedInputFeedbackConfig += consumer.ConsumeInputFeedbackConfig;
            InputFeedback.FeedInputFeedbackFrame += consumer.ConsumeInputFeedbackFrame;
            _consumers.Add(consumer);
        }

        public bool RemoveConsumer(Consumer.IConsumer consumer)
        {
            Audio.FeedAudioFormat -= consumer.ConsumeAudioFormat;
            Audio.FeedAudioData -= consumer.ConsumeAudioData;
            Video.FeedVideoFormat -= consumer.ConsumeVideoFormat;
            Video.FeedVideoData -= consumer.ConsumeVideoData;

            InputFeedback.FeedInputFeedbackConfig -= consumer.ConsumeInputFeedbackConfig;
            InputFeedback.FeedInputFeedbackFrame -= consumer.ConsumeInputFeedbackFrame;
            return _consumers.Remove(consumer);
        }

        public bool AddProvider(Provider.IProvider provider)
        {
            if (_provider != null)
            {
                Debug.WriteLine("Already got a provider!");
                return false;
            }
            _provider = provider;

            _provider.FeedInputConfig += Input.OnInputConfigReceived;
            _provider.FeedInputFrame += Input.OnInputFrameReceived;
            _provider.FeedChatAudioFormat += ChatAudio.OnChatAudioConfigReceived;
            _provider.FeedChatAudioData += ChatAudio.OnChatAudioDataReceived;
            return true;
        }

        public bool RemoveProvider()
        {
            if (_provider == null)
            {
                Debug.WriteLine("Got no provider to remove!");
                return false;
            }

            _provider.FeedInputConfig -= Input.OnInputConfigReceived;
            _provider.FeedInputFrame -= Input.OnInputFrameReceived;
            _provider.FeedChatAudioFormat -= ChatAudio.OnChatAudioConfigReceived;
            _provider.FeedChatAudioData -= ChatAudio.OnChatAudioDataReceived;

            _provider = null;
            return true;
        }

        public void Dispose()
        {
            _transport.Dispose();
        }
    }
}
