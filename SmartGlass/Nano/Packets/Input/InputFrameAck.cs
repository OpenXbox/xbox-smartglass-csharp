using System;
using System.IO;
using DarkId.SmartGlass.Common;
using DarkId.SmartGlass.Nano;

namespace DarkId.SmartGlass.Nano.Packets
{
    [InputPayloadType(InputPayloadType.FrameAck)]
    internal class InputFrameAck : ISerializableLE
    {
        public uint AckedFrame { get; private set; }

        public InputFrameAck()
        {
        }
        
        public InputFrameAck(uint ackedFrame)
        {
            AckedFrame = ackedFrame;
        }

        public void Deserialize(LEReader br)
        {
            AckedFrame = br.ReadUInt32();
        }

        public void Serialize(LEWriter bw)
        {
            bw.Write(AckedFrame);
        }
    }
}