using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [ControlOpCode(ControlOpCode.ChangeVideoQuality)]
    public class ChangeVideoQuality : StreamerMessageWithHeader
    {
        public uint Unknown1 { get; private set; }
        public uint Unknown2 { get; private set; }
        public uint Unknown3 { get; private set; }
        public uint Unknown4 { get; private set; }
        public uint Unknown5 { get; private set; }
        public uint Unknown6 { get; private set; }

        public ChangeVideoQuality()
            : base(ControlOpCode.ChangeVideoQuality)
        {
        }

        public ChangeVideoQuality(uint unk1, uint unk2, uint unk3,
                                  uint unk4, uint unk5, uint unk6)
            : base(ControlOpCode.ChangeVideoQuality)
        {
            Unknown1 = unk1;
            Unknown2 = unk2;
            Unknown3 = unk3;
            Unknown4 = unk4;
            Unknown5 = unk5;
            Unknown6 = unk6;
        }

        internal override void DeserializeStreamer(BinaryReader reader)
        {
            Unknown1 = reader.ReadUInt32();
            Unknown2 = reader.ReadUInt32();
            Unknown3 = reader.ReadUInt32();
            Unknown4 = reader.ReadUInt32();
            Unknown5 = reader.ReadUInt32();
            Unknown6 = reader.ReadUInt32();
        }

        internal override void SerializeStreamer(BinaryWriter writer)
        {
            writer.Write(Unknown1);
            writer.Write(Unknown2);
            writer.Write(Unknown3);
            writer.Write(Unknown4);
            writer.Write(Unknown5);
            writer.Write(Unknown6);
        }

    }
}