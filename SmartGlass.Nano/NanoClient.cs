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

        public async Task InitializeProtocolAsync()
        {
            Task awaitChannels = WaitForChannelsAsync();

            ControlHandshake response = await SendControlHandshakeAsync(ConnectionId);
            if (response.Type != ControlHandshakeType.ACK)
            {
                throw new NotSupportedException(
                    $"Invalid ControlHandshake type received {response.Type}");
            }
            RemoteConnectionId = response.ConnectionId;

            await awaitChannels;
            await OpenChannelsAsync();
        }

        public async Task InitializeStreamAsync(AudioFormat audioFormat, VideoFormat videoFormat)
        {
            await Audio.SendClientHandshakeAsync(audioFormat);
            await Video.SendClientHandshakeAsync(videoFormat);
        }

        public async Task StartStreamAsync()
        {
            Task handshakeTask = SendUdpHandshakeAsync();

            await Video.StartStreamAsync();
            await Audio.StartStreamAsync();

            await handshakeTask;
        }

        private async Task WaitForChannelsAsync()
        {
            Task<VideoChannel> video = WaitForChannelOpenAsync<VideoChannel>();
            Task<AudioChannel> audio = WaitForChannelOpenAsync<AudioChannel>();
            Task<ChatAudioChannel> chatAudio = WaitForChannelOpenAsync<ChatAudioChannel>();
            Task<ControlChannel> control = WaitForChannelOpenAsync<ControlChannel>();

            await Task.WhenAll(video, audio, chatAudio, control);

            Video = video.Result;
            Audio = audio.Result;
            ChatAudio = chatAudio.Result;
            Control = control.Result;
        }

        private async Task OpenChannelsAsync()
        {
            Task video = Video.OpenAsync();
            Task audio = Audio.OpenAsync();
            Task chatAudio = ChatAudio.OpenAsync();
            Task control = Control.OpenAsync();

            await Task.WhenAll(video, audio, chatAudio, control);
        }

        private async Task<TChannel> WaitForChannelOpenAsync<TChannel>()
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
            var packet = new Packets.ControlHandshake(type, connectionId)
            {
                Channel = NanoChannel.TcpBase
            };

            return await WaitForMessageAsync<ControlHandshake>(
                TimeSpan.FromSeconds(1),
                async () => await SendOnControlSocketAsync(packet));
        }

        private async Task SendUdpHandshakeAsync(
            ControlHandshakeType type = ControlHandshakeType.ACK)
        {
            var packet = new Packets.UdpHandshake(type)
            {
                Channel = NanoChannel.TcpBase
            };

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

            //InputFeedback.FeedInputFeedbackConfig += consumer.ConsumeInputFeedbackConfig;
            //InputFeedback.FeedInputFeedbackFrame += consumer.ConsumeInputFeedbackFrame;
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
