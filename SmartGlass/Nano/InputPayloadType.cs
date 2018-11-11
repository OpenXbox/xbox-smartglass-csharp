using System;
namespace DarkId.SmartGlass.Nano
{
    public enum InputPayloadType : uint
    {
        ServerHandshake = 1,
        ClientHandshake,
        FrameAck,
        Frame
    }
}
