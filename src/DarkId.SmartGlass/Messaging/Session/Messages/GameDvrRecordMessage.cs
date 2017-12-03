using System;
using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.GameDvrRecord)]
    internal class GameDvrRecordMessage : SessionMessageBase
    {
        public int StartTimeDelta { get; set; }
        public int EndTimeDelta { get; set; }

        public GameDvrRecordMessage()
        {
            Header.RequestAcknowledge = true;
        }

        public override void Deserialize(BEReader reader)
        {
            throw new NotSupportedException();
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write(StartTimeDelta);
            writer.Write(EndTimeDelta);
        }
    }
}