using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.MediaCommandResult)]
    internal class MediaCommandResultMessage : SessionMessageBase
    {
        public ulong RequestId { get; set; }
        public uint Result { get; set; }

        public override void Deserialize(BEReader reader)
        {
            RequestId = reader.ReadUInt64();
            Result = reader.ReadUInt32();
        }

        public override void Serialize(BEWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}
