using System;
namespace DarkId.SmartGlass.Nano
{
    public enum VideoPayloadType : uint
    {
        ServerHandshake = 1,
        ClientHandshake,
        Control,
        Data
    }
}
