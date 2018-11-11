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

        public override void Deserialize(BEReader reader)
        {
            TextSessionId = reader.ReadUInt64();
            TextBufferVersion = reader.ReadUInt32();
            TextOptions = (TextOption)reader.ReadUInt32();
            InputScope = (TextInputScope)reader.ReadUInt32();
            MaxTextLength = reader.ReadUInt32();
            Locale = reader.ReadString();
            Prompt = reader.ReadString();
        }

        public override void Serialize(BEWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}
