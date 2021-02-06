using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.TitleTextInput)]
    internal record TitleTextInput : SessionMessageBase
    {
        public ulong TextSessionId { get; set; }
        public uint TextBufferVersion { get; set; }
        public TextResult Result { get; set; }
        public string Text { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            TextSessionId = reader.ReadUInt64BE();
            TextBufferVersion = reader.ReadUInt32BE();
            Result = (TextResult)reader.ReadUInt16BE();
            Text = reader.ReadUInt16BEPrefixedString();
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.WriteBE(TextSessionId);
            writer.WriteBE(TextBufferVersion);
            writer.WriteBE((ushort)Result);
            writer.WriteUInt16BEPrefixed(Text);
        }
    }
}
