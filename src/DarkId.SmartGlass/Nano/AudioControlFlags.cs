using System;
namespace DarkId.SmartGlass.Nano
{
    [Flags]
    public enum AudioControlFlags
    {
        Reinitialize = 0x02,
        StartStream = 0x08,
        StopStream = 0x10
    }
}
