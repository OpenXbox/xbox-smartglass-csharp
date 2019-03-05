namespace SmartGlass.Messaging.Session
{
    /// <summary>
    /// Session message header.
    /// </summary>
    internal class SessionMessageHeader
    {
        public SessionMessageType SessionMessageType { get; set; }
        public bool RequestAcknowledge { get; set; }
        public bool IsFragment { get; set; }
        public ushort Version { get; set; }
        public ulong ChannelId { get; set; }
    }
}
