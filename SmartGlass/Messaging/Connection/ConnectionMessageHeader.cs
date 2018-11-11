using SmartGlass.Common;

namespace SmartGlass.Messaging.Connection
{
    internal class ConnectionMessageHeader : IProtectedMessageHeader
    {
        public MessageType Type { get; set; }

        public ushort PayloadLength { get; set; }

        public ushort ProtectedPayloadLength { get; set; }

        public ushort Version { get; set; } = 2;

        public void Deserialize(BEReader reader)
        {
            Type = (MessageType)reader.ReadUInt16();
            PayloadLength = reader.ReadUInt16();
            ProtectedPayloadLength = reader.ReadUInt16();
            Version = reader.ReadUInt16();
        }

        public void Serialize(BEWriter writer)
        {
            writer.Write((ushort)Type);
            writer.Write(PayloadLength);
            writer.Write(ProtectedPayloadLength);
            writer.Write(Version);
        }
    }
}