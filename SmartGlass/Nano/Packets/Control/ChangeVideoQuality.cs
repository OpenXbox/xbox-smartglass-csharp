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

        internal override void DeserializeStreamer(EndianReader reader)
        {
            Unknown1 = reader.ReadUInt32LE();
            Unknown2 = reader.ReadUInt32LE();
            Unknown3 = reader.ReadUInt32LE();
            Unknown4 = reader.ReadUInt32LE();
            Unknown5 = reader.ReadUInt32LE();
            Unknown6 = reader.ReadUInt32LE();
        }

        internal override void SerializeStreamer(EndianWriter writer)
        {
            writer.WriteLE(Unknown1);
            writer.WriteLE(Unknown2);
            writer.WriteLE(Unknown3);
            writer.WriteLE(Unknown4);
            writer.WriteLE(Unknown5);
            writer.WriteLE(Unknown6);
        }

    }
}