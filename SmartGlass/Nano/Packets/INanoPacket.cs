using System;
using SmartGlass.Common;

namespace SmartGlass.Nano.Packets
{
    public interface INanoPacket : ISerializable
    {
        NanoChannel Channel { get; set; }
        RtpHeader Header { get; set; }
    }
}