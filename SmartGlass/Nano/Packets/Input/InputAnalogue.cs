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

        public float GetValue(NanoGamepadAxis axis)
        {
            switch (axis)
            {
                case NanoGamepadAxis.LeftX:
                    return LeftThumbX;
                case NanoGamepadAxis.LeftY:
                    return LeftThumbY;
                case NanoGamepadAxis.RightX:
                    return RightThumbX;
                case NanoGamepadAxis.RightY:
                    return RightThumbY;
                case NanoGamepadAxis.TriggerLeft:
                    return LeftTrigger;
                case NanoGamepadAxis.TriggerRight:
                    return RightTrigger;
                default:
                    throw new NotSupportedException();
            }
        }

        public void SetValue(NanoGamepadAxis axis, float value)
        {
            switch (axis)
            {
                case NanoGamepadAxis.LeftX:
                    LeftThumbX = (ushort)value;
                    break;
                case NanoGamepadAxis.LeftY:
                    LeftThumbY = (ushort)value;
                    break;
                case NanoGamepadAxis.RightX:
                    RightThumbX = (ushort)value;
                    break;
                case NanoGamepadAxis.RightY:
                    RightThumbY = (ushort)value;
                    break;
                case NanoGamepadAxis.TriggerLeft:
                    LeftTrigger = (byte)((value / 32768) * 0xFF);
                    break;
                case NanoGamepadAxis.TriggerRight:
                    RightTrigger = (byte)((value / 32768) * 0xFF);
                    break;
                default:
                    throw new NotSupportedException();
            }
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
