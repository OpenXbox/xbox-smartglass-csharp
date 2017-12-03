using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.PowerOff)]
    internal class PowerOffMessage : SessionMessageBase
    {
        public string LiveId { get; set; }

        public override void Deserialize(BEReader reader)
        {
            throw new System.NotImplementedException();
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write(LiveId);
        }
    }
}