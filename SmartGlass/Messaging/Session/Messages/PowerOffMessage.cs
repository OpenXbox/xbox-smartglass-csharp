using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.PowerOff)]
    internal record PowerOffMessage : SessionMessageBase
    {
        public string LiveId { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            throw new System.NotImplementedException();
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.WriteUInt16BEPrefixed(LiveId);
        }
    }
}