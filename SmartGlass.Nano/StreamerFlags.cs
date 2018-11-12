using System;
namespace SmartGlass.Nano
{
    [Flags]
    public enum StreamerFlags : uint
    {
        GotSeqAndPrev = 0x1,
        Unknown1 = 0x2
    }
}