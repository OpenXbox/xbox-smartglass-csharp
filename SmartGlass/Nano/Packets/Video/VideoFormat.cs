using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    public class VideoFormat : ISerializableLE
    {
        public uint FPS { get; private set; }
        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public VideoCodec Codec { get; private set; }
        public uint Bpp { get; private set; }
        public uint Bytes { get; private set; }
        public ulong RedMask { get; private set; }
        public ulong GreenMask { get; private set; }
        public ulong BlueMask { get; private set; }

        public VideoFormat()
        {
        }

        public VideoFormat(uint fps, uint width, uint height, VideoCodec codec,
                           uint bpp = 0, uint bytes = 0, uint redMask = 0, uint greenMask = 0, uint blueMask = 0)
        {
            FPS = fps;
            Width = width;
            Height = height;
            Codec = codec;
            Bpp = bpp;
            Bytes = bytes;
            RedMask = redMask;
            GreenMask = greenMask;
            BlueMask = blueMask;
        }

        void ISerializableLE.Deserialize(BinaryReader reader)
        {
            FPS = reader.ReadUInt32();
            Width = reader.ReadUInt32();
            Height = reader.ReadUInt32();
            Codec = (VideoCodec)reader.ReadUInt32();
            if (Codec == VideoCodec.RGB)
            {
                Bpp = reader.ReadUInt32();
                Bytes = reader.ReadUInt32();
                RedMask = reader.ReadUInt32();
                GreenMask = reader.ReadUInt32();
                BlueMask = reader.ReadUInt32();
            }
        }

        void ISerializableLE.Serialize(BinaryWriter writer)
        {
            writer.Write(FPS);
            writer.Write(Width);
            writer.Write(Height);
            writer.Write((uint)Codec);
            if (Codec == VideoCodec.RGB)
            {
                writer.Write(Bpp);
                writer.Write(Bytes);
                writer.Write(RedMask);
                writer.Write(GreenMask);
                writer.Write(BlueMask);
            }
        }
    }
}
