using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.GameDvrRecord)]
    internal record GameDvrRecordMessage : SessionMessageBase
    {
        public int StartTimeDelta { get; set; }
        public int EndTimeDelta { get; set; }

        public GameDvrRecordMessage()
        {
            Header.RequestAcknowledge = true;
        }

        public override void Deserialize(EndianReader reader)
        {
            throw new NotSupportedException();
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.WriteBE(StartTimeDelta);
            writer.WriteBE(EndTimeDelta);
        }
    }
}