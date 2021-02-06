using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.StopChannel)]
    internal record StopChannelMessage : SessionMessageBase
    {
        public StopChannelMessage()
        {
            Header.RequestAcknowledge = true;
        }

        public ulong ChannelIdToStop { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            throw new NotSupportedException();
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.WriteBE(ChannelIdToStop);
        }
    }
}