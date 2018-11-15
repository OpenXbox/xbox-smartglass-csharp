using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.TitleTextInput)]
    internal class TitleTextInput : SessionMessageBase
    {
        public ulong TextSessionId { get; set; }
        public uint TextBufferVersion { get; set; }
        public TextResult Result { get; set; }
        public string Text { get; set; }

        public override void Deserialize(BEReader reader)
        {
            TextSessionId = reader.ReadUInt64();
            TextBufferVersion = reader.ReadUInt32();
            Result = (TextResult)reader.ReadUInt16();
            Text = reader.ReadUInt16PrefixedString();
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write(TextSessionId);
            writer.Write(TextBufferVersion);
            writer.Write((ushort)Result);
            writer.WriteUInt16Prefixed(Text);
        }
    }
}
