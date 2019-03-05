namespace SmartGlass
{
    /// <summary>
    /// Media type.
    /// Used by MediaChannel.
    /// </summary>
    public enum MediaType : ushort
    {
        NoMedia = 0x00,
        Music = 0x01,
        Video = 0x02,
        Image = 0x03,
        Conversation = 0x04,
        Game = 0x05
    }
}
