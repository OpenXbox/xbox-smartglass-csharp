using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
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