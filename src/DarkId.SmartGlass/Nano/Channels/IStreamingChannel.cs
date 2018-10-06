using System;
using DarkId.SmartGlass.Nano.Packets;

namespace DarkId.SmartGlass.Nano.Channels
{
    internal interface IStreamingChannel
    {
        void OnStreamer(Streamer streamer);
    }
}