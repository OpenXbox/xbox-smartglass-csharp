namespace DarkId.SmartGlass.Analysis
{
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