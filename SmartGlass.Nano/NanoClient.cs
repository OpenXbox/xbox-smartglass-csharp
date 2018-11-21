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
            SessionId = sessionId;
            ConnectionId = (ushort)new Random().Next(5000);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="audioFormat"></param>
        /// <param name="videoFormat"></param>
        /// <returns></returns>
        public async Task InitializeStreamAsync(AudioFormat audioFormat, VideoFormat videoFormat)
        {
            await Audio.SendClientHandshakeAsync(audioFormat);
            await Video.SendClientHandshakeAsync(videoFormat);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task InitializeInputChannel(uint desktopWidth, uint desktopHeight)
        {
            InputChannel tmpInput = new InputChannel(this, new byte[] { });
            InputFeedbackChannel tmpInputFeedback =
                new InputFeedbackChannel(this, new byte[] { }, desktopWidth, desktopHeight);

            await Task.WhenAll(tmpInput.OpenAsync(), tmpInputFeedback.OpenAsync());

            Input = tmpInput;
            InputFeedback = tmpInputFeedback;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task InitializeChatAudioChannel(AudioFormat audioFormat)
        {
            ChatAudioChannel tmpChatAudio = new ChatAudioChannel(
                this, new byte[0], audioFormat);

            await tmpChatAudio.OpenAsync();
            ChatAudio = tmpChatAudio;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task StartStreamAsync()
        {
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

            Video = new VideoChannel(this, video.Result.Flags);
            Audio = new AudioChannel(this, audio.Result.Flags);
            // ChatAudio channel is opened separately
            // ChatAudio = new ChatAudioChannel(this, audio.Result.Flags);
            Control = new ControlChannel(this, audio.Result.Flags);
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
            return await WaitForMessageAsync<ChannelOpen>(
                TimeSpan.FromSeconds(3),
                startAction: null,
                filter: open => open.Channel == channel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        internal async Task SendChannelOpenAsync(NanoChannel channel, byte[] flags)
        {
            var packet = new Nano.Packets.ChannelOpen(flags);
            packet.Channel = channel;
            await SendOnControlSocketAsync(packet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        internal async Task SendChannelCloseAsync(NanoChannel channel, uint reason)
        {
            var packet = new Nano.Packets.ChannelClose(reason);
            packet.Channel = channel;
            await SendOnControlSocketAsync(packet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        internal void MessageReceived(object sender, MessageReceivedEventArgs<INanoPacket> message)
        {
            var packet = message.Message;
            NanoPayloadType pt = packet.Header.PayloadType;

            Debug.WriteLine($"NANO: Received {pt} on Channel <{packet.Channel}>");

            IStreamerMessage streamerPacket = packet as IStreamerMessage;
            if (streamerPacket != null)
                GetChannelById(packet.Channel).OnPacket(streamerPacket);
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

            return await WaitForMessageAsync<ControlHandshake>(
                TimeSpan.FromSeconds(1),
                async () => await SendOnControlSocketAsync(packet));
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
                WaitForMessageAsync<VideoData>(
                    TimeSpan.FromSeconds(1),
                    async () => await SendOnStreamingSocketAsync(packet)
                ), UdpHandshakeRetries);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="startAction"></param>
        /// <returns></returns>
        internal Task<INanoPacket> WaitForMessageAsync(TimeSpan timeout, Action startAction)
        {
            return _transport.WaitForMessageAsync<INanoPacket, INanoPacket>(timeout, startAction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="startAction"></param>
        /// <param name="filter"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal Task<T> WaitForMessageAsync<T>(TimeSpan timeout, Action startAction, Func<T, bool> filter = null)
            where T : INanoPacket
        {
            return _transport.WaitForMessageAsync<T, INanoPacket>(timeout, startAction, filter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        internal async Task SendOnStreamingSocketAsync(INanoPacket packet)
        {
            packet.Header.ConnectionId = RemoteConnectionId;
            await _transport.SendAsyncStreaming(packet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
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
