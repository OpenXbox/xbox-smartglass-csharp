using System;
using SmartGlass.Common;

namespace SmartGlass.Nano.Packets
{
    public interface INanoPacket : ISerializable
    {
        RtpHeader Header { get; set; }
    }
}