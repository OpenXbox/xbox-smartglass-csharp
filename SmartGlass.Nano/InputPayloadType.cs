using System;
namespace SmartGlass.Nano
{
    public enum InputPayloadType : uint
    {
        ServerHandshake = 1,
        ClientHandshake,
        FrameAck,
        Frame
    }
}
