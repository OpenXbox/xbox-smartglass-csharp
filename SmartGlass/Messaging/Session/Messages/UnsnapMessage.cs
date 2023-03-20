using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.Unsnap)]
    internal record UnsnapMessage : SessionMessageBase
    {
        public byte Unknown { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            Unknown = reader.ReadByte();
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.Write(Unknown);
        }
    }
}
