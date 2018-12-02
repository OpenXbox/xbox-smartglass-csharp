using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    public class InputAnalogue : ISerializableLE
    {
        public byte LeftTrigger { get; set; }
        public byte RightTrigger { get; set; }
        public ushort LeftThumbX { get; set; }
        public ushort LeftThumbY { get; set; }
        public ushort RightThumbX { get; set; }
        public ushort RightThumbY { get; set; }
        public byte RumbleTriggerL { get; set; }
        public byte RumbleTriggerR { get; set; }
        public byte RumbleHandleL { get; set; }
        public byte RumbleHandleR { get; set; }

        public InputAnalogue()
        {
            LeftTrigger = 0;
            RightTrigger = 0;
            LeftThumbX = 0;
            LeftThumbY = 0;
            RightThumbX = 0;
            RightThumbY = 0;
            RumbleTriggerL = 0;
            RumbleTriggerR = 0;
            RumbleHandleL = 0;
            RumbleHandleR = 0;
        }

        void ISerializableLE.Deserialize(BinaryReader reader)
        {
            LeftTrigger = reader.ReadByte();
            RightTrigger = reader.ReadByte();
            LeftThumbX = reader.ReadByte();
            LeftThumbY = reader.ReadByte();
            RightThumbX = reader.ReadByte();
            RightThumbY = reader.ReadByte();
            RumbleTriggerL = reader.ReadByte();
            RumbleTriggerR = reader.ReadByte();
            RumbleHandleL = reader.ReadByte();
            RumbleHandleR = reader.ReadByte();
        }

        void ISerializableLE.Serialize(BinaryWriter writer)
        {
            writer.Write(LeftTrigger);
            writer.Write(RightTrigger);
            writer.Write(LeftThumbX);
            writer.Write(LeftThumbY);
            writer.Write(RightThumbX);
            writer.Write(RightThumbY);
            writer.Write(RumbleTriggerL);
            writer.Write(RumbleTriggerR);
            writer.Write(RumbleHandleL);
            writer.Write(RumbleHandleR);
        }
    }
}
