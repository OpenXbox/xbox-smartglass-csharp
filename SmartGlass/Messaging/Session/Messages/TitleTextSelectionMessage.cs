using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.TitleTextSelection)]
    internal record TitleTextSelection : SessionMessageBase
    {
        public ulong TextSessionId { get; set; }
        public uint TextBufferVersion { get; set; }
        public uint Start { get; set; }
        public uint Length { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            TextSessionId = reader.ReadUInt64BE();
            TextBufferVersion = reader.ReadUInt32BE();
            Start = reader.ReadUInt32BE();
            Length = reader.ReadUInt32BE();
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.WriteBE(TextSessionId);
            writer.WriteBE(TextBufferVersion);
            writer.WriteBE(Start);
            writer.WriteBE(Length);
        }
    }
}
