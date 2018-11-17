using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [InputPayloadType(InputPayloadType.FrameAck)]
    public class InputFrameAck : StreamerMessage
    {
        public uint AckedFrame { get; private set; }

        public InputFrameAck()
            : base((uint)InputPayloadType.FrameAck)
        {
        }

        public InputFrameAck(uint ackedFrame)
            : base((uint)InputPayloadType.FrameAck)
        {
            AckedFrame = ackedFrame;
        }

        public override void DeserializeStreamer(BinaryReader reader)
        {
            AckedFrame = reader.ReadUInt32();
        }

        public override void SerializeStreamer(BinaryWriter writer)
        {
            writer.Write(AckedFrame);
        }
    }
}