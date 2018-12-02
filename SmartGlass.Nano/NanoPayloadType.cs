using System;
namespace SmartGlass.Nano
{
    public enum NanoPayloadType : byte
    {
        Streamer = 0x23,
        ControlHandshake = 0x60,
        ChannelControl = 0x61,
        UDPHandshake = 0x64
    }
}
