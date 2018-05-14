using System;
namespace DarkId.SmartGlass.Nano
{
    public enum VideoPayloadType
    {
        ServerHandshake = 1,
        ClientHandshake,
        Control,
        Data
    }
}
