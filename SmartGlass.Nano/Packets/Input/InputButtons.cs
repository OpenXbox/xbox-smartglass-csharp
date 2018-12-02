using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    public class InputButtons : ISerializableLE
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

        void ISerializableLE.Deserialize(BinaryReader reader)
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

        void ISerializableLE.Serialize(BinaryWriter writer)
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
