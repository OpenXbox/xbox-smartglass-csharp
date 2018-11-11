using System;
namespace SmartGlass.Nano
{
    [Flags]
    public enum VideoControlFlags : uint
    {
        LastDisplayedFrame = 0x1,
        LostFrames = 0x2,
        QueueDepth = 0x4,
        StopStream = 0x8,
        StartStream = 0x10,
        RequestKeyframe = 0x20
    }
}
