using System;
namespace DarkId.SmartGlass.Nano
{
    public enum AudioPayloadType
    {
        ServerHandshake = 1,
        ClientHandshake,
        Control,
        Data
    }
}
