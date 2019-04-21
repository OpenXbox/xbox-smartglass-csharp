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

        internal override void DeserializeStreamer(EndianReader reader)
        {
            AckedFrame = reader.ReadUInt32LE();
        }

        internal override void SerializeStreamer(EndianWriter writer)
        {
            writer.WriteLE(AckedFrame);
        }
    }
}