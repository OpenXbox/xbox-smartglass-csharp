using System;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    public interface IStreamingChannel
    {
        void OnPacket(IStreamerMessage packet);
    }
}