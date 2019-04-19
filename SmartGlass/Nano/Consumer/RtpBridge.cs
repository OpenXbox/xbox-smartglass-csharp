using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SmartGlass.Common;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Consumer
{
    public class RtpBridgeConsumer : IConsumer, IDisposable
    {
        public const byte AudioPayloadType = 96;
        public const byte VideoPayloadType = 97;

        private bool _disposed = false;

        private readonly UdpClient _udpClient;
        public IPAddress MulticastAddress { get; private set; }
        public IPEndPoint AudioEndpoint { get; private set; }
        public IPEndPoint VideoEndpoint { get; private set; }
        public uint SSRC { get; private set; }

        public RtpBridgeConsumer(string multicastAddress = "224.1.1.1")
        {
            _udpClient = new UdpClient();
            MulticastAddress = IPAddress.Parse(multicastAddress);
            AudioEndpoint = new IPEndPoint(MulticastAddress, 5007);
            VideoEndpoint = new IPEndPoint(MulticastAddress, 6007);
            SSRC = (uint)new Random().Next(int.MaxValue);

            _udpClient.JoinMulticastGroup(MulticastAddress);
        }

        public string GetSdp()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("SDP:");
            sb.AppendLine("v=0");
            sb.AppendLine($"c=IN IP4 {MulticastAddress}");

            // Video
            sb.AppendLine($"m=video {VideoEndpoint.Port} RTP/AVP {VideoPayloadType}");
            sb.AppendLine($"a=rtpmap:{VideoPayloadType} H264/90000");
            sb.AppendLine($"a=fmtp:{VideoPayloadType} packetization-mode=1");

            // Audio
            sb.AppendLine($"m=audio {AudioEndpoint.Port} RTP/AVP {AudioPayloadType}");
            sb.AppendLine($"a=rtpmap:{AudioPayloadType} MPEG4-GENERIC/48000/2");
            sb.AppendLine($"a=fmtp:{AudioPayloadType} profile-level-id=1;mode=AAC-hbr;sizelength=13;indexlength=3;indexdeltalength=3");

            return sb.ToString();
        }

        int SendMulticast(byte[] data, IPEndPoint ep)
        {
            return _udpClient.Send(data, data.Length, ep);
        }

        void IVideoConsumer.ConsumeVideoData(object sender, VideoDataEventArgs args)
        {
            var packetData = args.VideoData;

            packetData.Header.ChannelId = (ushort)(SSRC & 0xFFFF);
            packetData.Header.ConnectionId = (ushort)(SSRC >> 16 & 0xFFFF);
            packetData.Header.Padding = false;
            packetData.Header.PayloadType = (NanoPayloadType)VideoPayloadType;
            // packetData.Header.Timestamp = (uint)(videoData.Timestamp & 0xFFFFFFFF);

            var writer = new BEWriter();
            packetData.Header.Serialize(writer);
            writer.Write(packetData.Data);

            SendMulticast(writer.ToBytes(), VideoEndpoint);
        }

        void IAudioConsumer.ConsumeAudioData(object sender, AudioDataEventArgs args)
        {
            var packetData = args.AudioData;

            packetData.Header.ChannelId = (ushort)(SSRC & 0xFFFF);
            packetData.Header.ConnectionId = (ushort)(SSRC >> 16 & 0xFFFF);
            packetData.Header.Padding = false;
            packetData.Header.PayloadType = (NanoPayloadType)AudioPayloadType;
            // packetData.Header.Timestamp = (uint)(audioData.Timestamp & 0xFFFFFFFF);

            var auHeader = new byte[4];
            auHeader[0] = 0;
            auHeader[1] = 0x10;
			auHeader[2] = (byte) (packetData.Data.Length >> 5);
			auHeader[3] = (byte) (packetData.Data.Length << 3);
            auHeader[3] &= 0xF8;
			auHeader[3] |= 0x00;

            var writer = new BEWriter();
            packetData.Header.Serialize(writer);
            writer.Write(auHeader);
            writer.Write(packetData.Data);

            SendMulticast(writer.ToBytes(), AudioEndpoint);
        }

        public void ConsumeInputFeedbackFrame(object sender, InputFrameEventArgs args)
        {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _udpClient.Dispose();
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
