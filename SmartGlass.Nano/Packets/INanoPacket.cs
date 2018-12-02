using System;
using SmartGlass.Common;

namespace SmartGlass.Nano.Packets
{
    public interface INanoPacket : ISerializableLE
    {
        NanoChannel Channel { get; set; }
        RtpHeader Header { get; set; }
    }
}