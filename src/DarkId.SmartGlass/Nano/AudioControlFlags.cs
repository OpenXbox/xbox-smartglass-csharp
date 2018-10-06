using System;
namespace DarkId.SmartGlass.Nano
{
    [Flags]
    public enum AudioControlFlags : uint
    {
        StopStream = 0x8,
        StartStream = 0x10,
        Reinitialize = 0x20
    }
}
