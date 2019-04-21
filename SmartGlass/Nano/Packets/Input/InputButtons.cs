using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    public class InputButtons : ISerializable
    {
        public byte DPadUp { get; set; }
        public byte DPadDown { get; set; }
        public byte DPadLeft { get; set; }
        public byte DPadRight { get; set; }
        public byte Start { get; set; }
        public byte Back { get; set; }
        public byte LeftThumbstick { get; set; }
        public byte RightThumbstick { get; set; }
        public byte LeftShoulder { get; set; }
        public byte RightShoulder { get; set; }
        public byte Guide { get; set; }
        public byte Unknown { get; set; }
        public byte A { get; set; }
        public byte B { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }

        public InputButtons()
        {
            DPadUp = 0;
            DPadDown = 0;
            DPadLeft = 0;
            DPadRight = 0;
            Start = 0;
            Back = 0;
            LeftThumbstick = 0;
            RightThumbstick = 0;
            LeftShoulder = 0;
            RightShoulder = 0;
            Guide = 0;
            Unknown = 0;
            A = 0;
            B = 0;
            X = 0;
            Y = 0;
        }

        public byte GetValue(NanoGamepadButton button)
        {
            switch (button)
            {
                case NanoGamepadButton.A:
                    return A;
                case NanoGamepadButton.B:
                    return B;
                case NanoGamepadButton.X:
                    return X;
                case NanoGamepadButton.Y:
                    return Y;
                case NanoGamepadButton.Start:
                    return Start;
                case NanoGamepadButton.Back:
                    return Back;
                case NanoGamepadButton.Guide:
                    return Guide;
                case NanoGamepadButton.DPadDown:
                    return DPadDown;
                case NanoGamepadButton.DPadLeft:
                    return DPadLeft;
                case NanoGamepadButton.DPadRight:
                    return DPadRight;
                case NanoGamepadButton.DPadUp:
                    return DPadUp;
                case NanoGamepadButton.LeftShoulder:
                    return LeftShoulder;
                case NanoGamepadButton.LeftThumbstick:
                    return LeftThumbstick;
                case NanoGamepadButton.RightShoulder:
                    return RightShoulder;
                case NanoGamepadButton.RightThumbstick:
                    return RightThumbstick;
                default:
                    throw new NotSupportedException();
            }
        }

        public void SetValue(NanoGamepadButton button, byte value)
        {
            switch (button)
            {
                case NanoGamepadButton.A:
                    A = value;
                    break;
                case NanoGamepadButton.B:
                    B = value;
                    break;
                case NanoGamepadButton.X:
                    X = value;
                    break;
                case NanoGamepadButton.Y:
                    Y = value;
                    break;
                case NanoGamepadButton.Start:
                    Start = value;
                    break;
                case NanoGamepadButton.Back:
                    Back = value;
                    break;
                case NanoGamepadButton.Guide:
                    Guide = value;
                    break;
                case NanoGamepadButton.DPadDown:
                    DPadDown = value;
                    break;
                case NanoGamepadButton.DPadLeft:
                    DPadLeft = value;
                    break;
                case NanoGamepadButton.DPadRight:
                    DPadRight = value;
                    break;
                case NanoGamepadButton.DPadUp:
                    DPadUp = value;
                    break;
                case NanoGamepadButton.LeftShoulder:
                    LeftShoulder = value;
                    break;
                case NanoGamepadButton.LeftThumbstick:
                    LeftThumbstick = value;
                    break;
                case NanoGamepadButton.RightShoulder:
                    RightShoulder = value;
                    break;
                case NanoGamepadButton.RightThumbstick:
                    RightThumbstick = value;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public void ToggleButton(NanoGamepadButton button, bool pressed)
        {
            byte currentValue = GetValue(button);
            bool currentlyPressed = !((currentValue % 2) == 0 || currentValue == 0);

            if (currentlyPressed == pressed)
                return;

            // Toggle button
            if (currentValue == 0xFF)
            {
                // Wrap around
                currentValue = 0;
            }
            else
            {
                ++currentValue;
            }

            SetValue(button, currentValue);
        }

        void ISerializable.Deserialize(EndianReader reader)
        {
            DPadUp = reader.ReadByte();
            DPadDown = reader.ReadByte();
            DPadLeft = reader.ReadByte();
            DPadRight = reader.ReadByte();
            Start = reader.ReadByte();
            Back = reader.ReadByte();
            LeftThumbstick = reader.ReadByte();
            RightThumbstick = reader.ReadByte();
            LeftShoulder = reader.ReadByte();
            RightShoulder = reader.ReadByte();
            Guide = reader.ReadByte();
            Unknown = reader.ReadByte();
            A = reader.ReadByte();
            B = reader.ReadByte();
            X = reader.ReadByte();
            Y = reader.ReadByte();
        }

        void ISerializable.Serialize(EndianWriter writer)
        {
            writer.Write(DPadUp);
            writer.Write(DPadDown);
            writer.Write(DPadLeft);
            writer.Write(DPadRight);
            writer.Write(Start);
            writer.Write(Back);
            writer.Write(LeftThumbstick);
            writer.Write(RightThumbstick);
            writer.Write(LeftShoulder);
            writer.Write(RightShoulder);
            writer.Write(Guide);
            writer.Write(Unknown);
            writer.Write(A);
            writer.Write(B);
            writer.Write(X);
            writer.Write(Y);
        }
    }
}
