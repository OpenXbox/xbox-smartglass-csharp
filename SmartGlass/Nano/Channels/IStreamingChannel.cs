using System;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    internal interface IStreamingChannel
    {
        void OnStreamer(Streamer streamer);
    }
}