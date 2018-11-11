using System;
using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.SystemTextDone)]
    internal class SystemTextDoneMessage : SessionMessageBase
	{
        public uint TextSessionId;
        public uint TextVersion;
        public uint Flags;
        public TextResult Result;

        public SystemTextDoneMessage()
		{
            Header.RequestAcknowledge = true;
		}

		public override void Deserialize(BEReader reader)
        {
            TextSessionId = reader.ReadUInt32();
            TextVersion = reader.ReadUInt32();
            Flags = reader.ReadUInt32();
            Result = (TextResult)reader.ReadUInt32();
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write(TextSessionId);
            writer.Write(TextVersion);
            writer.Write(Flags);
            writer.Write((uint)Result);
        }
	}
}