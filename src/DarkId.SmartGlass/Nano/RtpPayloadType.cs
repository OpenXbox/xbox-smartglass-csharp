using System;
namespace DarkId.SmartGlass.Nano
{
    public enum RtpPayloadType
    {
        Streamer = 0x23,
        Control = 0x60,
        ChannelControl = 0x61,
        UDPHandshake = 0x64
    }
}
