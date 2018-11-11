using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.MediaControllerRemoved)]
    internal class MediaControllerRemovedMessage : SessionMessageBase
    {
        public uint TitleId { get; set; }

        public override void Deserialize(BEReader reader)
        {
            TitleId = reader.ReadUInt32();
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write(TitleId);
        }
    }
}
