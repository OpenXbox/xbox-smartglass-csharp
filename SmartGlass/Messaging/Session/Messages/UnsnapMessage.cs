using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.Unsnap)]
    internal class UnsnapMessage : SessionMessageBase
    {
        public byte Unknown { get; set; }

        public override void Deserialize(BEReader reader)
        {
            Unknown = reader.ReadByte();
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write(Unknown);
        }
    }
}
