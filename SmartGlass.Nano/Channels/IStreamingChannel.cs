using System;
using SmartGlass.Common;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    public interface IStreamingChannel
    {
        void OnMessage(object sender, MessageReceivedEventArgs<INanoPacket> args);
    }
}