using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    public class StreamerHeader : ISerializable
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

        public void Deserialize(EndianReader br)
        {
            Flags = (StreamerFlags)br.ReadUInt32LE();
            if (Flags.HasFlag(StreamerFlags.GotSeqAndPrev))
            {
                SequenceNumber = br.ReadUInt32LE();
                PreviousSequenceNumber = br.ReadUInt32LE();
            }
            PacketType = br.ReadUInt32LE();
        }

        public void Serialize(EndianWriter bw)
        {
            bw.WriteLE((uint)Flags);
            if (Flags.HasFlag(StreamerFlags.GotSeqAndPrev))
            {
                bw.WriteLE(SequenceNumber);
                bw.WriteLE(PreviousSequenceNumber);
            }
            bw.WriteLE(PacketType);
        }
    }
}