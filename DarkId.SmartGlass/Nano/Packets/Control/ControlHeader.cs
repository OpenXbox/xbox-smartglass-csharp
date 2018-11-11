using System;
using System.IO;
using DarkId.SmartGlass.Common;
using DarkId.SmartGlass.Nano;

namespace DarkId.SmartGlass.Nano.Packets
{
    internal class ControlHeader : ISerializableLE
    {
        public uint PreviousSequence { get; private set; }
        public uint Unknown1 { get; private set; }
        public uint Unknown2 { get; private set; }
        public ControlOpCode OpCode { get; private set; }

        public ControlHeader()
        {
        }
        
        public ControlHeader(uint previousSequence, ushort unknown1,
                             ushort unknown2, ControlOpCode opCode)
        {
            PreviousSequence = previousSequence;
            Unknown1 = unknown1;
            Unknown2 = unknown2;
            OpCode = opCode;
        }

        public void Deserialize(LEReader br)
        {
            PreviousSequence = br.ReadUInt32();
            Unknown1 = br.ReadUInt16();
            Unknown2 = br.ReadUInt16();
            OpCode = (ControlOpCode)br.ReadUInt16();
        }

        public void Serialize(LEWriter bw)
        {
            bw.Write(PreviousSequence);
            bw.Write(Unknown1);
            bw.Write(Unknown2);
            bw.Write((ushort)OpCode);
        }
    }
}