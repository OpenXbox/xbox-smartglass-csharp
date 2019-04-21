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

        public void Deserialize(EndianReader reader)
        {
            Type = (MessageType)reader.ReadUInt16BE();
            PayloadLength = reader.ReadUInt16BE();
            SequenceNumber = reader.ReadUInt32BE();
            TargetParticipantId = reader.ReadUInt32BE();
            SourceParticipantId = reader.ReadUInt32BE();

            var typeInfo = reader.ReadUInt16BE();

            SessionMessageType = (SessionMessageType)(typeInfo & 0xFFF);
            IsFragment = (typeInfo & (1 << 12)) != 0;
            RequestAcknowledge = (typeInfo & (1 << 13)) != 0;
            Version = (ushort)((typeInfo >> 14) & 0xF);

            ChannelId = reader.ReadUInt64BE();
        }

        public void Serialize(EndianWriter writer)
        {
            writer.WriteBE((ushort)Type);
            writer.WriteBE(PayloadLength);
            writer.WriteBE(SequenceNumber);
            writer.WriteBE(TargetParticipantId);
            writer.WriteBE(SourceParticipantId);

            writer.WriteBE((ushort)((ushort)SessionMessageType +
                ((IsFragment ? 1 : 0) << 12) +
                ((RequestAcknowledge ? 1 : 0) << 13) +
                (Version << 14)));

            writer.WriteBE(ChannelId);
        }
    }
}
