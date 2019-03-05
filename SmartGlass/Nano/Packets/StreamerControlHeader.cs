using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    public class StreamerControlHeader : ISerializableLE
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

        public void Deserialize(BinaryReader br)
        {
            PreviousSequence = br.ReadUInt32();
            Unknown1 = br.ReadUInt16();
            Unknown2 = br.ReadUInt16();
            OpCode = (ControlOpCode)br.ReadUInt16();
        }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write(PreviousSequence);
            bw.Write(Unknown1);
            bw.Write(Unknown2);
            bw.Write((ushort)OpCode);
        }
    }
}