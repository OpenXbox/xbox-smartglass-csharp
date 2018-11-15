using SmartGlass.Common;

namespace SmartGlass.Messaging.Power
{
    [MessageType(MessageType.PowerOn)]
    internal class PowerOnMessage : MessageBase<PowerOnMessageHeader>
    {
        public string LiveId { get; set; }

        protected override void DeserializePayload(BEReader reader)
        {
            LiveId = reader.ReadUInt16PrefixedString();
        }

        protected override void SerializePayload(BEWriter writer)
        {
            writer.WriteUInt16Prefixed(LiveId);
        }
    }
}
