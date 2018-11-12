using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    internal class InputButtons : ISerializableLE
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

        public void Deserialize(LEReader br)
        {
            DPadUp = br.ReadByte();
            DPadDown = br.ReadByte();
            DPadLeft = br.ReadByte();
            DPadRight = br.ReadByte();
            Start = br.ReadByte();
            Back = br.ReadByte();
            LeftThumbstick = br.ReadByte();
            RightThumbstick = br.ReadByte();
            LeftShoulder = br.ReadByte();
            RightShoulder = br.ReadByte();
            Guide = br.ReadByte();
            Unknown = br.ReadByte();
            A = br.ReadByte();
            B = br.ReadByte();
            X = br.ReadByte();
            Y = br.ReadByte();
        }

        public void Serialize(LEWriter bw)
        {
            bw.Write(DPadUp);
            bw.Write(DPadDown);
            bw.Write(DPadLeft);
            bw.Write(DPadRight);
            bw.Write(Start);
            bw.Write(Back);
            bw.Write(LeftThumbstick);
            bw.Write(RightThumbstick);
            bw.Write(LeftShoulder);
            bw.Write(RightShoulder);
            bw.Write(Guide);
            bw.Write(Unknown);
            bw.Write(A);
            bw.Write(B);
            bw.Write(X);
            bw.Write(Y);
        }
    }
}
