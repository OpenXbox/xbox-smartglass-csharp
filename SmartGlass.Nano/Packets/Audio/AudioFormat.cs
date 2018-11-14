using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    public class AudioFormat : ISerializableLE
    {
        public uint Channels { get; private set; }
        public uint SampleRate { get; private set; }
        public AudioCodec Codec { get; private set; }
        public uint SampleSize { get; private set; }
        public uint SampleType { get; private set; }

        public AudioFormat()
        {
        }
        
        public AudioFormat(uint channels, uint sampleRate, AudioCodec codec,
                           uint sampleSize = 0, uint sampleType = 0)
        {
            Channels = channels;
            SampleRate = sampleRate;
            Codec = codec;
            SampleSize = sampleSize;
            SampleType = sampleType;
        }

        void ISerializableLE.Deserialize(BinaryReader br)
        {
            Channels = br.ReadUInt32();
            SampleRate = br.ReadUInt32();
            Codec = (AudioCodec)br.ReadUInt32();
            if (Codec == AudioCodec.PCM)
            {
                SampleSize = br.ReadUInt32();
                SampleType = br.ReadUInt32();
            }
        }

        void ISerializableLE.Serialize(BinaryWriter bw)
        {
            bw.Write(Channels);
            bw.Write(SampleRate);
            bw.Write((uint)Codec);
            if (Codec == AudioCodec.PCM)
            {
                bw.Write(SampleSize);
                bw.Write(SampleType);
            }
        }
    }
}
