using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    public class InputExtension : ISerializable
    {
        public byte Unknown1 { get; set; } // Always 1 for Gamepad!
        public byte Unknown2 { get; set; }
        public byte RumbleTriggerL { get; set; }
        public byte RumbleTriggerR { get; set; }
        public byte RumbleHandleL { get; set; }
        public byte RumbleHandleR { get; set; }
        public byte Unknown3 { get; set; }
        public byte Unknown4 { get; set; }
        public byte Unknown5 { get; set; }

        public InputExtension()
        {
            Unknown1 = 0;
            Unknown2 = 0;
            RumbleTriggerL = 0;
            RumbleTriggerR = 0;
            RumbleHandleL = 0;
            RumbleHandleR = 0;
            Unknown3 = 0;
            Unknown4 = 0;
            Unknown5 = 0;
        }

        void ISerializable.Deserialize(EndianReader reader)
        {
            Unknown1 = reader.ReadByte();
            Unknown2 = reader.ReadByte();
            RumbleTriggerL = reader.ReadByte();
            RumbleTriggerR = reader.ReadByte();
            RumbleHandleL = reader.ReadByte();
            RumbleHandleR = reader.ReadByte();
            Unknown3 = reader.ReadByte();
            Unknown4 = reader.ReadByte();
            Unknown5 = reader.ReadByte();
        }

        void ISerializable.Serialize(EndianWriter writer)
        {
            writer.Write(Unknown1);
            writer.Write(Unknown2);
            writer.Write(RumbleTriggerL);
            writer.Write(RumbleTriggerR);
            writer.Write(RumbleHandleL);
            writer.Write(RumbleHandleR);
            writer.Write(Unknown3);
            writer.Write(Unknown4);
            writer.Write(Unknown5);
        }
    }
}
