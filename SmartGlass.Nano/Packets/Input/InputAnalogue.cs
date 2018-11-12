using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    internal class InputAnalogue : ISerializableLE
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

        public void Deserialize(LEReader br)
        {
            LeftTrigger = br.ReadByte();
            RightTrigger = br.ReadByte();
            LeftThumbX = br.ReadByte();
            LeftThumbY = br.ReadByte();
            RightThumbX = br.ReadByte();
            RightThumbY = br.ReadByte();
            RumbleTriggerL = br.ReadByte();
            RumbleTriggerR = br.ReadByte();
            RumbleHandleL = br.ReadByte();
            RumbleHandleR = br.ReadByte();
        }

        public void Serialize(LEWriter bw)
        {
            bw.Write(LeftTrigger);
            bw.Write(RightTrigger);
            bw.Write(LeftThumbX);
            bw.Write(LeftThumbY);
            bw.Write(RightThumbX);
            bw.Write(RightThumbY);
            bw.Write(RumbleTriggerL);
            bw.Write(RumbleTriggerR);
            bw.Write(RumbleHandleL);
            bw.Write(RumbleHandleR);
        }
    }
}
