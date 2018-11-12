using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [ControlOpCode(ControlOpCode.RealtimeTelemetry)]
    internal class RealtimeTelemetry : ISerializableLE
    {
        public RealtimeTelemetry()
        {
        }

        public void Deserialize(LEReader br)
        {
        }

        public void Serialize(LEWriter bw)
        {
        }
    }
}