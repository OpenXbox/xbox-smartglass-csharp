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
        public const byte VideoPayloadType = 96;
        public const byte AudioPayloadType = 97;

        private bool _disposed = false;

        private readonly UdpClient _udpClient;
        public IPAddress MulticastAddress { get; private set; }
        public IPEndPoint AudioEndpoint { get; private set; }
        public IPEndPoint VideoEndpoint { get; private set; }
        public ushort SSRC { get; private set; }

        public RtpBridgeConsumer(string multicastAddress = "224.1.1.1")
        {
            _udpClient = new UdpClient();
            MulticastAddress = IPAddress.Parse(multicastAddress);
            AudioEndpoint = new IPEndPoint(MulticastAddress, 5890);
            VideoEndpoint = new IPEndPoint(MulticastAddress, 5892);
            SSRC = (ushort)new Random().Next(ushort.MaxValue);

            _udpClient.JoinMulticastGroup(MulticastAddress);
        }

        public string GetSdp()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("v=0");
            sb.AppendLine($"c=IN IP4 {MulticastAddress}");

            // Video
            sb.AppendLine($"m=video {VideoEndpoint.Port} RTP/AVP {VideoPayloadType}");
            sb.AppendLine($"a=rtpmap:{VideoPayloadType} H264/90000");

            // Audio
            /*
            sb.AppendLine($"m=audio {AudioEndpoint.Port} RTP/AVP {AudioPayloadType}");
            sb.AppendLine($"a=rtpmap:{AudioPayloadType} mpeg4-generic/48000");
            sb.AppendLine($"a=fmtp:{AudioPayloadType} streamType=5;profile-level-id=44;mode=AAC-hbr;sizelength=13;indexlength=3;indexdeltalength=3");
            */

            return sb.ToString();
        }

        int SendMulticast(byte[] data, IPEndPoint ep)
        {
            return _udpClient.Send(data, data.Length, ep);
        }

        void IVideoConsumer.ConsumeVideoData(object sender, VideoDataEventArgs args)
        {
            var videoData = args.VideoData;

            videoData.Header.ChannelId = 0;
            videoData.Header.ConnectionId = SSRC;
            videoData.Header.Padding = false;
            videoData.Header.PayloadType = (NanoPayloadType)VideoPayloadType;

            var writer = new BEWriter();
            videoData.Header.Serialize(writer);
            writer.Write(videoData.Data);

            SendMulticast(writer.ToBytes(), VideoEndpoint);
        }

        void IAudioConsumer.ConsumeAudioData(object sender, AudioDataEventArgs args)
        {
            var audioData = args.AudioData;

            audioData.Header.ChannelId = 0;
            audioData.Header.ConnectionId = SSRC;
            audioData.Header.Padding = false;
            audioData.Header.PayloadType = (NanoPayloadType)AudioPayloadType;

            var writer = new BEWriter();
            audioData.Header.Serialize(writer);
            writer.Write(audioData.Data);

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
