using System;
namespace DarkId.SmartGlass.Nano
{
    public enum AudioPayloadType : uint
    {
        ServerHandshake = 1,
        ClientHandshake,
        Control,
        Data
    }
}
