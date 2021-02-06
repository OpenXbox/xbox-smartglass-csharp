using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.MediaCommandResult)]
    internal record MediaCommandResultMessage : SessionMessageBase
    {
        public ulong RequestId { get; set; }
        public uint Result { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            RequestId = reader.ReadUInt64BE();
            Result = reader.ReadUInt32BE();
        }

        public override void Serialize(EndianWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}
