using SmartGlass.Common;

namespace SmartGlass.Messaging.Discovery
{
    /// <summary>
    /// Discovery message header, used by PresenceRequest and PresenceResponse.
    /// </summary>
    internal class DiscoveryMessageHeader : IMessageHeader
    {
        public MessageType Type { get; set; }

        public ushort PayloadLength { get; set; }

        public ushort Version { get; set; }

        public void Deserialize(EndianReader reader)
        {
            Type = (MessageType)reader.ReadUInt16BE();
            PayloadLength = reader.ReadUInt16BE();
            Version = reader.ReadUInt16BE();
        }

        public void Serialize(EndianWriter writer)
        {
            writer.WriteBE((ushort)Type);
            writer.WriteBE(PayloadLength);
            writer.WriteBE(Version);
        }
    }
}
