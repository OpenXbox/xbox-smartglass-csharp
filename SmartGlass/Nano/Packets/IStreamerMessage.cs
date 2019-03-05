using System;

namespace SmartGlass.Nano.Packets
{
    public interface IStreamerMessage : INanoPacket
    {
        StreamerHeader StreamerHeader { get; }
    }
}
