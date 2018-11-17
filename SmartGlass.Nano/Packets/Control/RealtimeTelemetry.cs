using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [ControlOpCode(ControlOpCode.RealtimeTelemetry)]
    public class RealtimeTelemetry : StreamerMessageWithHeader
    {
        public RealtimeTelemetry()
            : base(ControlOpCode.RealtimeTelemetry)
        {
        }

        public override void DeserializeStreamer(BinaryReader reader)
        {
        }

        public override void SerializeStreamer(BinaryWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}