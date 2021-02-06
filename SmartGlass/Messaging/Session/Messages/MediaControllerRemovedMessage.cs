using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.MediaControllerRemoved)]
    internal record MediaControllerRemovedMessage : SessionMessageBase
    {
        public uint TitleId { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            TitleId = reader.ReadUInt32BE();
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.WriteBE(TitleId);
        }
    }
}
