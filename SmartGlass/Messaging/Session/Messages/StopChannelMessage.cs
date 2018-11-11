using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.StopChannel)]
    internal class StopChannelMessage : SessionMessageBase
    {
        public StopChannelMessage()
        {
            Header.RequestAcknowledge = true;
        }

        public ulong ChannelIdToStop { get; set; }

        public override void Deserialize(BEReader reader)
        {
            throw new NotSupportedException();
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write(ChannelIdToStop);
        }
    }
}