using System;
namespace DarkId.SmartGlass.Nano
{
    [Flags]
    public enum VideoControlFlags
    {
        RequestKeyframe = 0x04,
        StartStream = 0x08,
        StopStream = 0x10,
        QueueDepth = 0x20,
        LostFrames = 0x40,
        LastDisplayedFrame = 0x80
    }
}
