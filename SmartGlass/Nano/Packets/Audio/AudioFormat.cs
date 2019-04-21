using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    public class AudioFormat : ISerializable
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

        void ISerializable.Deserialize(EndianReader reader)
        {
            Channels = reader.ReadUInt32LE();
            SampleRate = reader.ReadUInt32LE();
            Codec = (AudioCodec)reader.ReadUInt32LE();
            if (Codec == AudioCodec.PCM)
            {
                SampleSize = reader.ReadUInt32LE();
                SampleType = reader.ReadUInt32LE();
            }
        }

        void ISerializable.Serialize(EndianWriter writer)
        {
            writer.WriteLE(Channels);
            writer.WriteLE(SampleRate);
            writer.WriteLE((uint)Codec);
            if (Codec == AudioCodec.PCM)
            {
                writer.WriteLE(SampleSize);
                writer.WriteLE(SampleType);
            }
        }
    }
}
