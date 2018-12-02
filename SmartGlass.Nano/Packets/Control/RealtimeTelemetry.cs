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

        internal override void DeserializeStreamer(BinaryReader reader)
        {
        }

        internal override void SerializeStreamer(BinaryWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}