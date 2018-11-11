using System;
namespace SmartGlass.Nano
{
    public enum RtpPayloadType : byte
    {
        Streamer = 0x23,
        Control = 0x60,
        ChannelControl = 0x61,
        UDPHandshake = 0x64
    }
}
