using System;
namespace DarkId.SmartGlass.Nano
{
    public enum InputPayloadType
    {
        ServerHandshake = 1,
        ClientHandshake,
        FrameAck,
        Frame
    }
}
