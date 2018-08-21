using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DarkId.SmartGlass.Common;
using DarkId.SmartGlass.Nano;

namespace DarkId.SmartGlass
{
    public class NanoClient : IDisposable
    {
        private readonly NanoRdpTransport _transport;
        private readonly Nano.Channels.ChannelManager _channelManager;
        
        internal Nano.Consumer.IConsumer _consumer { get; set; }

        public Guid SessionId { get; internal set; }
        public ushort ConnectionId { get; private set; }
        public ushort RemoteConnectionId { get; private set; }

        public bool ControlHandshakeDone { get; internal set; }

        public NanoClient(string address, int tcpPort, int udpPort,
                          Guid sessionId)
        {
            _transport = new NanoRdpTransport(address, tcpPort, udpPort);
            _transport.MessageReceived += MessageReceived;
            _channelManager = new Nano.Channels.ChannelManager(this);
            ControlHandshakeDone = false;
            SessionId = sessionId;
            ConnectionId = (ushort)new Random().Next(0xFFFF);

            // For testing
            _consumer = new Nano.Consumer.FileConsumer("nanodump");
        }

        public void Initialize()
        {
            Debug.WriteLine("Starting NanoClient...");
            SendControlHandshake();

            Thread.Sleep(2000);
            StartStream();
        }

        public void StartStream()
        {
            if (!_channelManager.Video.HandshakeDone || !_channelManager.Audio.HandshakeDone)
            {
                Console.WriteLine("Audio or Video handshake not done yet, cant start stream");
            }
            _channelManager.Video.StartStream();
            _channelManager.Audio.StartStream();
            Task.Run(() =>
            {
                while (!_transport.udpDataActive)
                {
                    SendUdpHandshake();
                    Thread.Sleep(3000);
                }
            });
        }

        internal void MessageReceived(object sender, MessageReceivedEventArgs<RtpPacket> message)
        {
            var packet = message.Message;
            RtpPayloadType ptype = packet.Header.PayloadType;
            Debug.WriteLine($"Received {ptype}");
            switch (ptype)
            {
                case RtpPayloadType.Control:
                    OnControlHandshake(packet);
                    break;
                case RtpPayloadType.ChannelControl:
                    var controlMsg = packet.Payload as Nano.Packets.ChannelControl;
                    _channelManager.HandleChannelControl(controlMsg, packet.Header.ChannelId);
                    break;
                case RtpPayloadType.Streamer:
                    var streamer = packet.Payload as Nano.Packets.Streamer;
                    _channelManager.HandleStreamer(streamer, packet.Header.ChannelId);
                    break;
                case RtpPayloadType.UDPHandshake:
                    throw new NotSupportedException($"UDP HandshakeType from server received");
                default:
                    throw new NotSupportedException($"Unsupported payload type: {ptype}");
            }
        }

        internal void OnControlHandshake(RtpPacket packet)
        {
            var handshake = packet.Payload as Nano.Packets.ControlHandshake;
            if (handshake.Type != ControlHandshakeType.ACK)
            {
                throw new NotSupportedException($"Invalid Control HandshakeType received {handshake.Type}");
            }
            RemoteConnectionId = handshake.ConnectionId;
            ControlHandshakeDone = true;
        }

        internal void SendControlHandshake()
        {
            var payload = new Nano.Packets.ControlHandshake(ControlHandshakeType.SYN,
                                                            ConnectionId);
            var packet = new RtpPacket(RtpPayloadType.Control);
            packet.SetPayload(payload);

            SendOnControlSocket(packet);
        }

        internal void SendUdpHandshake()
        {
            var payload = new Nano.Packets.UdpHandshake(ControlHandshakeType.ACK);
            var packet = new RtpPacket(RtpPayloadType.UDPHandshake);
            packet.SetPayload(payload);

            SendOnStreamingSocket(packet);
        }

        internal async void SendOnStreamingSocket(RtpPacket packet)
        {
            packet.Header.ConnectionId = ConnectionId;
            await _transport.SendAsync(packet);
        }

        internal async void SendOnControlSocket(RtpPacket packet)
        {
            await _transport.SendAsync(packet);
        }

        public void Dispose()
        {
            _transport.Dispose();
        }
    }
}