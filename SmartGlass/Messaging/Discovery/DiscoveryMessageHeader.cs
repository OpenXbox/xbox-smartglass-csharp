using SmartGlass.Common;

namespace SmartGlass.Messaging.Discovery
{
    internal class DiscoveryMessageHeader : IMessageHeader
    {
        public MessageType Type { get; set; }

        public ushort PayloadLength { get; set; }

        public ushort Version { get; set; }

        public void Deserialize(BEReader reader)
        {
            Type = (MessageType)reader.ReadUInt16();
            PayloadLength = reader.ReadUInt16();
            Version = reader.ReadUInt16();
        }

        public void Serialize(BEWriter writer)
        {
            writer.Write((ushort)Type);
            writer.Write(PayloadLength);
            writer.Write(Version);
        }
    }
}