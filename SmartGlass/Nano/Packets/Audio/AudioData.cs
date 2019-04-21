using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [AudioPayloadType(AudioPayloadType.Data)]
    public class AudioData : StreamerMessage
    {
        public uint Flags { get; private set; }
        public uint FrameId { get; private set; }
        public ulong Timestamp { get; private set; }
        public byte[] Data { get; private set; }

        public AudioData()
            : base((uint)AudioPayloadType.Data)
        {
        }

        public AudioData(uint flags, uint frameId,
                         ulong timestamp, byte[] data)
            : base((uint)AudioPayloadType.Data)
        {
            Flags = flags;
            FrameId = frameId;
            Timestamp = timestamp;
            Data = data;
        }

        internal override void DeserializeStreamer(EndianReader reader)
        {
            Flags = reader.ReadUInt32LE();
            FrameId = reader.ReadUInt32LE();
            Timestamp = reader.ReadUInt64LE();
            Data = reader.ReadUInt32LEPrefixedBlob();
        }

        internal override void SerializeStreamer(EndianWriter writer)
        {
            writer.WriteLE(Flags);
            writer.WriteLE(FrameId);
            writer.WriteLE(Timestamp);
            writer.WriteUInt32LEPrefixedBlob(Data);
        }
    }
}
