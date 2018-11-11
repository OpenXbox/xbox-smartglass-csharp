using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.SystemTextAcknowledge)]
    internal class SystemTextAcknowledgeMessage : SessionMessageBase
	{
        public uint TextSessionId { get; set; }
        public uint TextVersionAck { get; set; }

        public SystemTextAcknowledgeMessage()
		{
            Header.RequestAcknowledge = true;
		}

        public override void Deserialize(BEReader reader)
		{
            TextSessionId = reader.ReadUInt32();
            TextVersionAck = reader.ReadUInt32();
		}

        public override void Serialize(BEWriter writer)
		{
            writer.Write(TextSessionId);
            writer.Write(TextVersionAck);
		}
	}
}