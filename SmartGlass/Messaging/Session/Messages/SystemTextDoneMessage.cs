using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.SystemTextDone)]
    internal record SystemTextDoneMessage : SessionMessageBase
    {
        public uint TextSessionId;
        public uint TextVersion;
        public uint Flags;
        public TextResult Result;

        public SystemTextDoneMessage()
        {
            Header.RequestAcknowledge = true;
        }

        public override void Deserialize(EndianReader reader)
        {
            TextSessionId = reader.ReadUInt32BE();
            TextVersion = reader.ReadUInt32BE();
            Flags = reader.ReadUInt32BE();
            Result = (TextResult)reader.ReadUInt32BE();
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.WriteBE(TextSessionId);
            writer.WriteBE(TextVersion);
            writer.WriteBE(Flags);
            writer.WriteBE((uint)Result);
        }
    }
}