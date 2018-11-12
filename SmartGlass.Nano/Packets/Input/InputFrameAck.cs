using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [InputPayloadType(InputPayloadType.FrameAck)]
    public class InputFrameAck : ISerializableLE
    {
        public uint AckedFrame { get; private set; }

        public InputFrameAck()
        {
        }

        public InputFrameAck(uint ackedFrame)
        {
            AckedFrame = ackedFrame;
        }

        void ISerializableLE.Deserialize(LEReader br)
        {
            AckedFrame = br.ReadUInt32();
        }

        void ISerializableLE.Serialize(LEWriter bw)
        {
            bw.Write(AckedFrame);
        }
    }
}