using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    public class InputExtension : ISerializableLE
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

        void ISerializableLE.Deserialize(LEReader br)
        {
            Unknown1 = br.ReadByte();
            Unknown2 = br.ReadByte();
            RumbleTriggerL = br.ReadByte();
            RumbleTriggerR = br.ReadByte();
            RumbleHandleL = br.ReadByte();
            RumbleHandleR = br.ReadByte();
            Unknown3 = br.ReadByte();
            Unknown4 = br.ReadByte();
            Unknown5 = br.ReadByte();
        }

        void ISerializableLE.Serialize(LEWriter bw)
        {
            bw.Write(Unknown1);
            bw.Write(Unknown2);
            bw.Write(RumbleTriggerL);
            bw.Write(RumbleTriggerR);
            bw.Write(RumbleHandleL);
            bw.Write(RumbleHandleR);
            bw.Write(Unknown3);
            bw.Write(Unknown4);
            bw.Write(Unknown5);
        }
    }
}
