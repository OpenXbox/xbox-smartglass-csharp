namespace SmartGlass
{
    /// <summary>
    /// Media playback status.
    /// Used by MediaChannel.
    /// </summary>
    public enum MediaPlaybackStatus : uint
    {
        Closed = 0x00,
        Changing = 0x01,
        Stopped = 0x02,
        Playing = 0x03,
        Paused = 0x04
    }
}
