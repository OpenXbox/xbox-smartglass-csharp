using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.Disconnect)]
    internal class DisconnectMessage : SessionMessageBase
    {
        public DisconnectReason Reason { get; set; }
        public uint ErrorCode { get; set; }

        public override void Deserialize(BEReader reader)
        {
            throw new System.NotImplementedException();
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write((uint)Reason);
            writer.Write(ErrorCode);
        }
    }
}