using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Messaging.Power
{
    [MessageType(MessageType.PowerOn)]
    internal class PowerOnMessage : MessageBase<PowerOnMessageHeader>
    {
        public string LiveId { get; set; }
        
        protected override void DeserializePayload(BEReader reader)
        {
            LiveId = reader.ReadString();
        }

        protected override void SerializePayload(BEWriter writer)
        {
            writer.Write(LiveId);
        }
    }
}
