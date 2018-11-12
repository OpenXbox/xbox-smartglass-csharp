using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [ControlOpCode(ControlOpCode.ChangeVideoQuality)]
    internal class ChangeVideoQuality : ISerializableLE
    {
        public uint Unknown1 { get; private set; }
        public uint Unknown2 { get; private set; }
        public uint Unknown3 { get; private set; }
        public uint Unknown4 { get; private set; }
        public uint Unknown5 { get; private set; }
        public uint Unknown6 { get; private set; }

        public ChangeVideoQuality(uint unk1, uint unk2, uint unk3,
                                  uint unk4, uint unk5, uint unk6)
        {
            Unknown1 = unk1;
            Unknown2 = unk2;
            Unknown3 = unk3;
            Unknown4 = unk4;
            Unknown5 = unk5;
            Unknown6 = unk6;
        }

        public void Deserialize(LEReader br)
        {
            Unknown1 = br.ReadUInt32();
            Unknown2 = br.ReadUInt32();
            Unknown3 = br.ReadUInt32();
            Unknown4 = br.ReadUInt32();
            Unknown5 = br.ReadUInt32();
            Unknown6 = br.ReadUInt32();
        }

        public void Serialize(LEWriter bw)
        {
            bw.Write(Unknown1);
            bw.Write(Unknown2);
            bw.Write(Unknown3);
            bw.Write(Unknown4);
            bw.Write(Unknown5);
            bw.Write(Unknown6);
        }
    }
}