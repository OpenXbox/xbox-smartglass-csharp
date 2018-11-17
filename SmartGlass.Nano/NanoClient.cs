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

            Control = new ControlChannel(this);
            Video = new VideoChannel(this);
            Audio = new AudioChannel(this);
            ChatAudio = new ChatAudioChannel(this);
            Input = new InputChannel(this);
            InputFeedback = new InputFeedbackChannel(this);
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
            var response = await _transport.WaitForMessageAsync<ControlHandshake>(
                TimeSpan.FromSeconds(1),
                async () => await SendControlHandshakeAsync(ConnectionId));

            if (response.Type != ControlHandshakeType.ACK)
            {
                throw new NotSupportedException(
                    $"Invalid ControlHandshake type received {response.Type}");
            }
            RemoteConnectionId = response.ConnectionId;
        }

        public async Task<bool> StartStream()
        {
            if (!Video.HandshakeComplete || !Audio.HandshakeComplete)
            {
                Console.WriteLine("Audio or Video handshake not done yet, cant start stream");
                return false;
            }

            Video.StartStream();
            Audio.StartStream();

            TimeSpan[] udpHandshakeRetries = new TimeSpan[]
            {
                TimeSpan.FromMilliseconds(100),
                TimeSpan.FromMilliseconds(200),
                TimeSpan.FromMilliseconds(400),
                TimeSpan.FromMilliseconds(800)
            };

            var response = await TaskExtensions.WithRetries(() =>
                _transport.WaitForMessageAsync<VideoData>(
                    TimeSpan.FromSeconds(1),
                    async () => await SendUdpHandshakeAsync()
                ), udpHandshakeRetries);

            return response != null;
        }

        internal void MessageReceived(object sender, MessageReceivedEventArgs<INanoPacket> message)
        {
            var packet = message.Message;
            NanoPayloadType pt = packet.Header.PayloadType;

            Debug.WriteLine($"NANO: Received {pt} on Channel <{packet.Channel}>");

            switch (pt)
            {
                case NanoPayloadType.ControlHandshake:
                    // Handled by this.Initialize()
                    break;
                case NanoPayloadType.ChannelControl:
                    OnChannelControlMessage(packet as Packets.ChannelControlMessage);
                    break;
                case NanoPayloadType.Streamer:
                    OnStreamer(packet as IStreamerMessage);
                    break;
                case NanoPayloadType.UDPHandshake:
                    throw new NotSupportedException($"UDP handshake received from server :/");
                default:
                    throw new NotSupportedException($"Unsupported payload type: {pt}");
            }
        }

        private void OnStreamer(IStreamerMessage msg)
        {
            GetChannelById(msg.Channel).OnPacket(msg);
        }

        private void OnChannelControlMessage(Packets.ChannelControlMessage packet)
        {
            if (ChannelControlType.Create == packet.Type)
                OnChannelCreate(packet as Packets.ChannelCreate);
            else if (ChannelControlType.Open == packet.Type)
                OnChannelOpen(packet as ChannelOpen);
            else if (ChannelControlType.Close == packet.Type)
                OnChannelClose(packet as ChannelClose);
            else
                throw new NanoException(
                    $"Invalid ChannelControl packet: {packet.Type}");
        }

        private void OnChannelCreate(Packets.ChannelCreate packet)
        {
            IStreamingChannel channel = GetChannelById(NanoChannelClass
                .GetIdByClassName(packet.Name));
            ((StreamingChannelBase)channel).Create(packet.Flags);
        }

        private void OnChannelOpen(Packets.ChannelOpen packet)
        {
            if (packet.Channel == NanoChannel.Unknown)
            {
                throw new NanoException(
                    $"Unknown channel was opened, id: {packet.Header.ChannelId}");
            }

            IStreamingChannel channel = GetChannelById(packet.Channel);
            ((StreamingChannelBase)channel).Open(packet.Flags);

            SendChannelOpenAsync(packet.Channel, packet.Flags)
                .GetAwaiter().GetResult();
        }

        private void OnChannelClose(Packets.ChannelClose packet)
        {
            IStreamingChannel channel = GetChannelById(packet.Channel);
            ((StreamingChannelBase)channel).Close(packet.Flags);

            SendChannelCloseAsync(packet.Channel, packet.Flags)
                .GetAwaiter().GetResult();
        }

        private Task SendControlHandshakeAsync(ushort connectionId,
            ControlHandshakeType type = ControlHandshakeType.SYN)
        {
            var packet = new Packets.ControlHandshake(type, connectionId);
            return SendOnControlSocketAsync(packet);
        }

        private Task SendUdpHandshakeAsync(
            ControlHandshakeType type = ControlHandshakeType.ACK)
        {
            var packet = new Packets.UdpHandshake(type);
            return SendOnStreamingSocketAsync(packet);
        }

        internal Task SendChannelOpenAsync(NanoChannel channel, byte[] flags)
        {
            var packet = new Nano.Packets.ChannelOpen(flags);
            packet.Channel = channel;
            return SendOnControlSocketAsync(packet);
        }

        internal Task SendChannelCloseAsync(NanoChannel channel, uint flags)
        {
            var packet = new Nano.Packets.ChannelClose(flags);
            packet.Channel = channel;
            return SendOnControlSocketAsync(packet);
        }

        internal Task SendOnStreamingSocketAsync(INanoPacket packet)
        {
            packet.Header.ConnectionId = RemoteConnectionId;
            return _transport.SendAsyncStreaming(packet);
        }

        internal Task SendOnControlSocketAsync(INanoPacket packet)
        {
            return _transport.SendAsyncControl(packet);
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
