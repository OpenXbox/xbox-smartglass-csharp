using System;
namespace SmartGlass.Nano
{
    public enum AudioPayloadType : uint
    {
        ServerHandshake = 1,
        ClientHandshake,
        Control,
        Data
    }
}
