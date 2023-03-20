using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.StartChannelResponse)]
    internal record StartChannelResponseMessage : SessionMessageBase
    {
        public uint ChannelRequestId { get; set; }
        public ulong ChannelId { get; set; }
        public int Result { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            ChannelRequestId = reader.ReadUInt32BE();
            ChannelId = reader.ReadUInt64BE();
            Result = reader.ReadInt32BE();
        }

        public override void Serialize(EndianWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}