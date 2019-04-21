using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    public class StreamerControlHeader : ISerializable
    {
        public uint PreviousSequence { get; set; }
        public ushort Unknown1 { get; set; }
        public ushort Unknown2 { get; set; }
        public ControlOpCode OpCode { get; private set; }

        public StreamerControlHeader()
        {
        }

        public StreamerControlHeader(ControlOpCode opCode)
        {
            OpCode = opCode;
        }

        public void Deserialize(EndianReader br)
        {
            PreviousSequence = br.ReadUInt32LE();
            Unknown1 = br.ReadUInt16LE();
            Unknown2 = br.ReadUInt16LE();
            OpCode = (ControlOpCode)br.ReadUInt16LE();
        }

        public void Serialize(EndianWriter bw)
        {
            bw.WriteLE(PreviousSequence);
            bw.WriteLE(Unknown1);
            bw.WriteLE(Unknown2);
            bw.WriteLE((ushort)OpCode);
        }
    }
}