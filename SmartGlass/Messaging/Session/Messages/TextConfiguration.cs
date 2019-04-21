using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    internal class TextConfiguration : SessionMessageBase
    {
        ulong TextSessionId { get; set; }
        uint TextBufferVersion { get; set; }
        TextOption TextOptions { get; set; }
        TextInputScope InputScope { get; set; }
        uint MaxTextLength { get; set; }
        string Locale { get; set; }
        string Prompt { get; set; }

        public TextConfiguration()
        {
            Header.RequestAcknowledge = true;
        }

        public override void Deserialize(EndianReader reader)
        {
            TextSessionId = reader.ReadUInt64BE();
            TextBufferVersion = reader.ReadUInt32BE();
            TextOptions = (TextOption)reader.ReadUInt32BE();
            InputScope = (TextInputScope)reader.ReadUInt32BE();
            MaxTextLength = reader.ReadUInt32BE();
            Locale = reader.ReadUInt16BEPrefixedString();
            Prompt = reader.ReadUInt16BEPrefixedString();
        }

        public override void Serialize(EndianWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}
