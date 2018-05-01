namespace DarkId.SmartGlass
{
    using System;

    [Flags]
    public enum MediaControlCommands : uint
    {
        None = 0x0000,
        Play = 0x0002,
        Pause = 0x0004,
        PlayPauseToggle = 0x0008,
        Stop = 0x0010,
        Record = 0x0020,
        NextTrack = 0x0040,
        PreviousTrack = 0x0080,
        FastForward = 0x0100,
        Rewind = 0x0200,
        ChannelUp = 0x0400,
        ChannelDown = 0x0800,
        Back = 0x1000,
        View = 0x2000,
        Menu = 0x4000,
        Seek = 0x8000,
    }
}
