using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.TitleTextSelection)]
    internal class TitleTextSelection : SessionMessageBase
    {
        public ulong TextSessionId { get; set; }
        public uint TextBufferVersion { get; set; }
        public uint Start { get; set; }
        public uint Length { get; set; }

        public override void Deserialize(BEReader reader)
        {
            TextSessionId = reader.ReadUInt64();
            TextBufferVersion = reader.ReadUInt32();
            Start = reader.ReadUInt32();
            Length = reader.ReadUInt32();
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write(TextSessionId);
            writer.Write(TextBufferVersion);
            writer.Write(Start);
            writer.Write(Length);
        }
    }
}
