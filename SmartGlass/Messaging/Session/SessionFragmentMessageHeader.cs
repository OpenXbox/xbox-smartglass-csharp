using SmartGlass.Common;

namespace SmartGlass.Messaging.Session
{
    /// <summary>
    /// Session fragment message header.
    /// </summary>
    internal class SessionFragmentMessageHeader : SessionMessageHeader, IMessageHeader
    {
        public MessageType Type { get; set; }

        public ushort PayloadLength { get; set; }

        public uint SequenceNumber { get; set; }

        public uint TargetParticipantId { get; set; }

        public uint SourceParticipantId { get; set; }

        public void Deserialize(BEReader reader)
        {
            Type = (MessageType)reader.ReadUInt16();
            PayloadLength = reader.ReadUInt16();
            SequenceNumber = reader.ReadUInt32();
            TargetParticipantId = reader.ReadUInt32();
            SourceParticipantId = reader.ReadUInt32();

            var typeInfo = reader.ReadUInt16();

            SessionMessageType = (SessionMessageType)(typeInfo & 0xFFF);
            IsFragment = (typeInfo & (1 << 12)) != 0;
            RequestAcknowledge = (typeInfo & (1 << 13)) != 0;
            Version = (ushort)((typeInfo >> 14) & 0xF);

            ChannelId = reader.ReadUInt64();
        }

        public void Serialize(BEWriter writer)
        {
            writer.Write((ushort)Type);
            writer.Write(PayloadLength);
            writer.Write(SequenceNumber);
            writer.Write(TargetParticipantId);
            writer.Write(SourceParticipantId);

            writer.Write((ushort)((ushort)SessionMessageType +
                ((IsFragment ? 1 : 0) << 12) +
                ((RequestAcknowledge ? 1 : 0) << 13) +
                (Version << 14)));

            writer.Write(ChannelId);
        }
    }
}
