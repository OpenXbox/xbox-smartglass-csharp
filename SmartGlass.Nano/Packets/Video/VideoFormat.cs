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

        void ISerializableLE.Deserialize(BinaryReader br)
        {
            FPS = br.ReadUInt32();
            Width = br.ReadUInt32();
            Height = br.ReadUInt32();
            Codec = (VideoCodec)br.ReadUInt32();
            if (Codec == VideoCodec.RGB)
            {
                Bpp = br.ReadUInt32();
                Bytes = br.ReadUInt32();
                RedMask = br.ReadUInt32();
                GreenMask = br.ReadUInt32();
                BlueMask = br.ReadUInt32();
            }
        }

        void ISerializableLE.Serialize(BinaryWriter bw)
        {
            bw.Write(FPS);
            bw.Write(Width);
            bw.Write(Height);
            bw.Write((uint)Codec);
            if (Codec == VideoCodec.RGB)
            {
                bw.Write(Bpp);
                bw.Write(Bytes);
                bw.Write(RedMask);
                bw.Write(GreenMask);
                bw.Write(BlueMask);
            }
        }
    }
}
