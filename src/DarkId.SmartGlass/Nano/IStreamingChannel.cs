using System;
using DarkId.SmartGlass.Nano.Packets;

namespace DarkId.SmartGlass.Nano
{
    internal interface IStreamingChannel
    {
        void OnStreamer(Streamer streamer);
    }
}