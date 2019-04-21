using SmartGlass.Common;

namespace SmartGlass.Messaging.Connection
{
    /// <summary>
    /// Message header used by ConnectRequest and ConnectResponse.
    /// </summary>
    internal class ConnectionMessageHeader : IProtectedMessageHeader
    {
        public MessageType Type { get; set; }

        public ushort PayloadLength { get; set; }

        public ushort ProtectedPayloadLength { get; set; }

        public ushort Version { get; set; } = 2;

        public void Deserialize(EndianReader reader)
        {
            Type = (MessageType)reader.ReadUInt16BE();
            PayloadLength = reader.ReadUInt16BE();
            ProtectedPayloadLength = reader.ReadUInt16BE();
            Version = reader.ReadUInt16BE();
        }

        public void Serialize(EndianWriter writer)
        {
            writer.WriteBE((ushort)Type);
            writer.WriteBE(PayloadLength);
            writer.WriteBE(ProtectedPayloadLength);
            writer.WriteBE(Version);
        }
    }
}
