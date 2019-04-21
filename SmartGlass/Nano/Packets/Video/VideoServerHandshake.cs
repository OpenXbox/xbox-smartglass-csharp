using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [VideoPayloadType(VideoPayloadType.ServerHandshake)]
    public class VideoServerHandshake : StreamerMessage
    {
        public uint ProtocolVersion { get; private set; }
        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public uint FPS { get; private set; }
        public ulong ReferenceTimestamp { get; private set; }
        public VideoFormat[] Formats { get; private set; }

        public VideoServerHandshake()
            : base((uint)VideoPayloadType.ServerHandshake)
        {
        }

        public VideoServerHandshake(uint protocolVersion,
                                    uint width, uint height,
                                    uint fps, ulong refTimestamp,
                                    VideoFormat[] formats)
            : base((uint)VideoPayloadType.ServerHandshake)
        {
            ProtocolVersion = protocolVersion;
            Width = width;
            Height = height;
            FPS = fps;
            ReferenceTimestamp = refTimestamp;
            Formats = formats;
        }

        internal override void DeserializeStreamer(EndianReader reader)
        {
            ProtocolVersion = reader.ReadUInt32LE();
            Width = reader.ReadUInt32LE();
            Height = reader.ReadUInt32LE();
            FPS = reader.ReadUInt32LE();
            ReferenceTimestamp = reader.ReadUInt64LE();
            Formats = reader.ReadUInt32LEPrefixedArray<VideoFormat>();
        }

        internal override void SerializeStreamer(EndianWriter writer)
        {
            writer.WriteLE(ProtocolVersion);
            writer.WriteLE(Width);
            writer.WriteLE(Height);
            writer.WriteLE(FPS);
            writer.WriteLE(ReferenceTimestamp);
            writer.WriteUInt32LEPrefixedArray(Formats);
        }
    }
}
