using System;
using SmartGlass.Common;

namespace SmartGlass.Nano.Packets
{
    public class RtpHeader : ISerializable
    {
        public int Version { get; set; }
        public bool Padding { get; set; }
        public bool Extension { get; set; }
        public int CsrcCount { get; set; }
        public bool Marker { get; set; }
        public NanoPayloadType PayloadType { get; set; }

        public ushort SequenceNumber { get; set; }
        public uint Timestamp { get; set; }
        public ushort ConnectionId { get; set; }
        public ushort ChannelId { get; set; }

        public RtpHeader()
        {
            Version = 2;
            Padding = false;
            Extension = false;
            CsrcCount = 0;
            Marker = false;
            PayloadType = (NanoPayloadType)0;
            SequenceNumber = 0;
            Timestamp = 0;
            ConnectionId = 0;
            ChannelId = 0;
        }
        public void Deserialize(EndianReader reader)
        {
            var flags = reader.ReadUInt16BE();

            Version = (flags & 0xC000) >> 14;
            Padding = (flags & 0x2000) != 0;
            Extension = (flags & 0x1000) != 0;
            CsrcCount = (flags & 0xF00) >> 8;
            Marker = (flags & 0x80) != 0;
            PayloadType = (NanoPayloadType)((flags) & 0x7F);

            SequenceNumber = reader.ReadUInt16BE();
            Timestamp = reader.ReadUInt32BE();
            ConnectionId = reader.ReadUInt16BE();
            ChannelId = reader.ReadUInt16BE();
        }

        public void Serialize(EndianWriter writer)
        {
            var flags = 0;

            flags |= (Version & 3) << 14;
            flags |= (Padding ? 1 : 0) << 13;
            flags |= (Extension ? 1 : 0) << 12;
            flags |= (CsrcCount & 0xF) << 8;
            flags |= (Marker ? 1 : 0) << 7;
            flags |= (byte)PayloadType & 0x7F;

            writer.WriteBE((ushort)flags);
            writer.WriteBE(SequenceNumber);
            writer.WriteBE(Timestamp);
            writer.WriteBE(ConnectionId);
            writer.WriteBE(ChannelId);
        }
    }
}
