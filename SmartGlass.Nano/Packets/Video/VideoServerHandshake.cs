using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [VideoPayloadType(VideoPayloadType.ServerHandshake)]
    internal class VideoServerHandshake : ISerializableLE
    {
        public uint ProtocolVersion { get; private set; }
        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public uint FPS { get; private set; }
        public ulong ReferenceTimestamp { get; private set; }
        public VideoFormat[] Formats { get; private set; }

        public VideoServerHandshake()
        {
        }

        public VideoServerHandshake(uint protocolVersion,
                                    uint width, uint height,
                                    uint fps, ulong refTimestamp,
                                    VideoFormat[] formats)
        {
            ProtocolVersion = protocolVersion;
            Width = width;
            Height = height;
            FPS = fps;
            ReferenceTimestamp = refTimestamp;
            Formats = formats;
        }

        public void Deserialize(BinaryReader br)
        {
            ProtocolVersion = br.ReadUInt32();
            Width = br.ReadUInt32();
            Height = br.ReadUInt32();
            FPS = br.ReadUInt32();
            ReferenceTimestamp = br.ReadUInt64();
            Formats = br.ReadUInt32PrefixedArray<VideoFormat>();
        }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write(ProtocolVersion);
            bw.Write(Width);
            bw.Write(Height);
            bw.Write(FPS);
            bw.Write(ReferenceTimestamp);
            bw.Write((uint)Formats.Length);
            foreach (ISerializableLE f in Formats)
            {
                f.Serialize(bw);
            }
        }
    }
}
