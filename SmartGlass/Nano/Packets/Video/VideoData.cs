using System;
using System.IO;
using DarkId.SmartGlass.Common;
using DarkId.SmartGlass.Nano;

namespace DarkId.SmartGlass.Nano.Packets
{
    [VideoPayloadType(VideoPayloadType.Data)]
    internal class VideoData : ISerializableLE
    {
        public uint Flags { get; private set; }
        public uint FrameId { get; private set; }
        public long Timestamp { get; private set; }
        public uint TotalSize { get; private set; }
        public uint PacketCount { get; private set; }
        public uint Offset { get; private set; }
        public byte[] Data { get; private set; }

        public VideoData()
        {
        }
        
        public VideoData(uint flags, uint frameId, long timestamp,
                         uint totalSize, uint packetCount,
                         uint offset, byte[] data)
        {
            Flags = flags;
            FrameId = frameId;
            Timestamp = timestamp;
            TotalSize = totalSize;
            PacketCount = packetCount;
            Offset = offset;
            Data = data;
        }

        public void Deserialize(LEReader br)
        {
            Flags = br.ReadUInt32();
            FrameId = br.ReadUInt32();
            Timestamp = br.ReadInt64();
            TotalSize = br.ReadUInt32();
            PacketCount = br.ReadUInt32();
            Offset = br.ReadUInt32();
            Data = br.ReadBlobUInt32();
        }

        public void Serialize(LEWriter bw)
        {
            bw.Write(Flags);
            bw.Write(FrameId);
            bw.Write(Timestamp);
            bw.Write(TotalSize);
            bw.Write(PacketCount);
            bw.Write(Offset);

            bw.Write((uint)Data.Length);
            bw.Write(Data);
        }
    }
}