using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    public class VideoFormat : ISerializable
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

        void ISerializable.Deserialize(EndianReader reader)
        {
            FPS = reader.ReadUInt32LE();
            Width = reader.ReadUInt32LE();
            Height = reader.ReadUInt32LE();
            Codec = (VideoCodec)reader.ReadUInt32LE();
            if (Codec == VideoCodec.RGB)
            {
                Bpp = reader.ReadUInt32LE();
                Bytes = reader.ReadUInt32LE();
                RedMask = reader.ReadUInt32LE();
                GreenMask = reader.ReadUInt32LE();
                BlueMask = reader.ReadUInt32LE();
            }
        }

        void ISerializable.Serialize(EndianWriter writer)
        {
            writer.WriteLE(FPS);
            writer.WriteLE(Width);
            writer.WriteLE(Height);
            writer.WriteLE((uint)Codec);
            if (Codec == VideoCodec.RGB)
            {
                writer.WriteLE(Bpp);
                writer.WriteLE(Bytes);
                writer.WriteLE(RedMask);
                writer.WriteLE(GreenMask);
                writer.WriteLE(BlueMask);
            }
        }
    }
}
