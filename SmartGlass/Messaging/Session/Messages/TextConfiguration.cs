using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    internal record TextConfiguration : SessionMessageBase
    {
        public ulong TextSessionId { get; set; }
        public uint TextBufferVersion { get; set; }
        public TextOption TextOptions { get; set; }
        public TextInputScope InputScope { get; set; }
        public uint MaxTextLength { get; set; }
        public string Locale { get; set; }
        public string Prompt { get; set; }

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
