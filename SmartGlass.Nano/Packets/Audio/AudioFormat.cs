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

        void ISerializableLE.Deserialize(BinaryReader reader)
        {
            Channels = reader.ReadUInt32();
            SampleRate = reader.ReadUInt32();
            Codec = (AudioCodec)reader.ReadUInt32();
            if (Codec == AudioCodec.PCM)
            {
                SampleSize = reader.ReadUInt32();
                SampleType = reader.ReadUInt32();
            }
        }

        void ISerializableLE.Serialize(BinaryWriter writer)
        {
            writer.Write(Channels);
            writer.Write(SampleRate);
            writer.Write((uint)Codec);
            if (Codec == AudioCodec.PCM)
            {
                writer.Write(SampleSize);
                writer.Write(SampleType);
            }
        }
    }
}
