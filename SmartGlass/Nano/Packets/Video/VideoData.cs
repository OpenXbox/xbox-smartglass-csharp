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

        internal override void DeserializeStreamer(BinaryReader reader)
        {
            Flags = reader.ReadUInt32();
            FrameId = reader.ReadUInt32();
            Timestamp = reader.ReadUInt64();
            TotalSize = reader.ReadUInt32();
            PacketCount = reader.ReadUInt32();
            Offset = reader.ReadUInt32();
            Data = reader.ReadUInt32PrefixedBlob();
        }

        internal override void SerializeStreamer(BinaryWriter writer)
        {
            writer.Write(Flags);
            writer.Write(FrameId);
            writer.Write(Timestamp);
            writer.Write(TotalSize);
            writer.Write(PacketCount);
            writer.Write(Offset);
            writer.WriteUInt32PrefixedBlob(Data);
        }
    }
}
