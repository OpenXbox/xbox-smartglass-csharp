using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    public class StreamerHeader
    {
        public StreamerFlags Flags { get; internal set; }
        public uint SequenceNumber { get; internal set; }
        public uint PreviousSequenceNumber { get; internal set; }
        public uint PacketType { get; internal set; }

        public StreamerHeader()
        {
        }

        public StreamerHeader(uint packetType)
        {
            PacketType = packetType;
        }

        public void Deserialize(BinaryReader br)
        {
            Flags = (StreamerFlags)br.ReadUInt32();
            if (Flags.HasFlag(StreamerFlags.GotSeqAndPrev))
            {
                SequenceNumber = br.ReadUInt32();
                PreviousSequenceNumber = br.ReadUInt32();
            }
            PacketType = br.ReadUInt32();
        }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write((uint)Flags);
            if (Flags.HasFlag(StreamerFlags.GotSeqAndPrev))
            {
                bw.Write(SequenceNumber);
                bw.Write(PreviousSequenceNumber);
            }
            bw.Write(PacketType);
        }
    }
}