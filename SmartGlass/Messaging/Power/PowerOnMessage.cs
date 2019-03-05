using SmartGlass.Common;

namespace SmartGlass.Messaging.Power
{
    /// <summary>
    /// Power on message.
    /// </summary>
    [MessageType(MessageType.PowerOn)]
    internal class PowerOnMessage : MessageBase<PowerOnMessageHeader>
    {
        /// <summary>
        /// Live ID of console to be powered on.
        /// </summary>
        /// <value>The live identifier.</value>
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
