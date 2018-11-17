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

        public override void DeserializeStreamer(BinaryReader reader)
        {
            ProtocolVersion = reader.ReadUInt32();
            Width = reader.ReadUInt32();
            Height = reader.ReadUInt32();
            FPS = reader.ReadUInt32();
            ReferenceTimestamp = reader.ReadUInt64();
            Formats = reader.ReadUInt32PrefixedArray<VideoFormat>();
        }

        public override void SerializeStreamer(BinaryWriter writer)
        {
            writer.Write(ProtocolVersion);
            writer.Write(Width);
            writer.Write(Height);
            writer.Write(FPS);
            writer.Write(ReferenceTimestamp);
            writer.WriteUInt32PrefixedArray(Formats);
        }
    }
}
