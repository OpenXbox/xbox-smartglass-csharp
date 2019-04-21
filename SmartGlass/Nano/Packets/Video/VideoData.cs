using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [VideoPayloadType(VideoPayloadType.Data)]
    public class VideoData : StreamerMessage
    {
        public uint Flags { get; private set; }
        public uint FrameId { get; private set; }
        public ulong Timestamp { get; private set; }
        public uint TotalSize { get; private set; }
        public uint PacketCount { get; private set; }
        public uint Offset { get; private set; }
        public byte[] Data { get; private set; }

        public VideoData()
            : base((uint)VideoPayloadType.Data)
        {
        }

        public VideoData(uint flags, uint frameId, ulong timestamp,
                         uint totalSize, uint packetCount,
                         uint offset, byte[] data)
            : base((uint)VideoPayloadType.Data)
        {
            Flags = flags;
            FrameId = frameId;
            Timestamp = timestamp;
            TotalSize = totalSize;
            PacketCount = packetCount;
            Offset = offset;
            Data = data;
        }

        internal override void DeserializeStreamer(EndianReader reader)
        {
            Flags = reader.ReadUInt32LE();
            FrameId = reader.ReadUInt32LE();
            Timestamp = reader.ReadUInt64LE();
            TotalSize = reader.ReadUInt32LE();
            PacketCount = reader.ReadUInt32LE();
            Offset = reader.ReadUInt32LE();
            Data = reader.ReadUInt32LEPrefixedBlob();
        }

        internal override void SerializeStreamer(EndianWriter writer)
        {
            writer.WriteLE(Flags);
            writer.WriteLE(FrameId);
            writer.WriteLE(Timestamp);
            writer.WriteLE(TotalSize);
            writer.WriteLE(PacketCount);
            writer.WriteLE(Offset);
            writer.WriteUInt32LEPrefixedBlob(Data);
        }
    }
}
