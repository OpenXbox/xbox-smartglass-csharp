namespace SmartGlass.Analysis
{
    /// <summary>
    /// MessageInfo holds basic information about a SmartGlass
    /// message in a flat representation.
    /// </summary>
    public class MessageInfo
    {
        public string MessageType { get; set; }
        public ulong ChannelId { get; set; }
        public int Version { get; set; }
        public byte[] Data { get; set; }
        public string Json { get; set; }
        public bool RequestAcknowledge { get; set; }
    }
}