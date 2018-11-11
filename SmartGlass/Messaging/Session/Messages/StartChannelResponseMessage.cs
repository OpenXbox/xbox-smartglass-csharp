using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.StartChannelResponse)]
    internal class StartChannelResponseMessage : SessionMessageBase
    {
        public uint ChannelRequestId { get; set; }
        public ulong ChannelId { get; set; }
        public int Result { get; set; }

        public override void Deserialize(BEReader reader)
        {
            ChannelRequestId = reader.ReadUInt32();
            ChannelId = reader.ReadUInt64();
            Result = reader.ReadInt32();
        }

        public override void Serialize(BEWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}