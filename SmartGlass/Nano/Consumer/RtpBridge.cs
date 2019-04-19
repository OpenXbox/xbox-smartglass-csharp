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

        public const byte BITMASK_NALU_TYPE = 0x1F;
        public const byte NALU_TYPE_SPS = 0x7;
        public const byte NALU_TYPE_PPS = 0x8;

        private bool _disposed = false;

        private bool _gotVideoParams = false;
        private byte[] _spsBytes = null;
        private byte[] _ppsBytes = null;
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

        bool ExtractSpsPps(byte[] nalu)
        {
            if (nalu[0] != 0x00 || nalu[1] != 0x00 || nalu[2] != 0x00 || nalu[3] != 0x01
                || (nalu[4] & BITMASK_NALU_TYPE) != NALU_TYPE_SPS)
            {
                return false;
            }

            int startPos = 4;
            for (int i=startPos; i < nalu.Length; i++)
            {
                if (nalu[i] == 0x00 && nalu[i + 1] == 0x00 && nalu[i + 2] == 0x00 && nalu[i + 3] == 0x01)
                {
                    _spsBytes = new byte[i - startPos];
                    Array.Copy(nalu, startPos, _spsBytes, 0, _spsBytes.Length);
                    break;
                }
            }

            if (_spsBytes == null && (_spsBytes[0] & BITMASK_NALU_TYPE) != NALU_TYPE_SPS)
            {
                Debug.WriteLine("SPS could not be determined");
                return false;
            }

            startPos += (_spsBytes.Length + 4);
            for (int i=startPos; i < nalu.Length; i++)
            {
                if (nalu[i] == 0x00 && nalu[i + 1] == 0x00 && nalu[i + 2] == 0x00 && nalu[i + 3] == 0x01)
                {
                    _ppsBytes = new byte[i - startPos];
                    Array.Copy(nalu, startPos, _ppsBytes, 0, _ppsBytes.Length);
                    break;
                }
            }

            if (_ppsBytes == null && (_ppsBytes[0] & BITMASK_NALU_TYPE) != NALU_TYPE_PPS)
            {
                Debug.WriteLine("PPS could not be determined");
                return false;
            }

            return true;
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

            var writer = new BEWriter();
            packetData.Header.Serialize(writer);
            writer.Write(packetData.Data);

            SendMulticast(writer.ToBytes(), VideoEndpoint);

            if (!_gotVideoParams)
                _gotVideoParams = ExtractSpsPps(packetData.Data);
        }

        void IAudioConsumer.ConsumeAudioData(object sender, AudioDataEventArgs args)
        {
            var packetData = args.AudioData;

            packetData.Header.ChannelId = (ushort)(SSRC & 0xFFFF);
            packetData.Header.ConnectionId = (ushort)(SSRC >> 16 & 0xFFFF);
            packetData.Header.Padding = false;
            packetData.Header.PayloadType = (NanoPayloadType)AudioPayloadType;

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
