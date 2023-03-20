using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.SystemTextAcknowledge)]
    internal record SystemTextAcknowledgeMessage : SessionMessageBase
    {
        public uint TextSessionId { get; set; }
        public uint TextVersionAck { get; set; }

        public SystemTextAcknowledgeMessage()
        {
            Header.RequestAcknowledge = true;
        }

        public override void Deserialize(EndianReader reader)
        {
            TextSessionId = reader.ReadUInt32BE();
            TextVersionAck = reader.ReadUInt32BE();
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.WriteBE(TextSessionId);
            writer.WriteBE(TextVersionAck);
        }
    }
}