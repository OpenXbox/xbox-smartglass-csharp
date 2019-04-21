using SmartGlass.Common;

namespace SmartGlass.Messaging.Discovery
{
    /// <summary>
    /// Presence request message.
    /// Send from client to multicast / broadcast.
    /// Active consoles will respond with PresenceResponse.
    /// </summary>
    [MessageType(MessageType.PresenceRequest)]
    internal class PresenceRequestMessage : MessageBase<DiscoveryMessageHeader>
    {
        public uint Flags { get; set; }
        public DeviceType DeviceType { get; set; } = DeviceType.WindowsStore;
        public ushort MinVersion { get; set; } = 0;
        public ushort MaxVersion { get; set; } = 2;

        protected override void DeserializePayload(EndianReader reader)
        {
            Flags = reader.ReadUInt32BE();
            DeviceType = (DeviceType)reader.ReadUInt16BE();
            MinVersion = reader.ReadUInt16BE();
            MaxVersion = reader.ReadUInt16BE();
        }

        protected override void SerializePayload(EndianWriter writer)
        {
            writer.WriteBE(Flags);
            writer.WriteBE((ushort)DeviceType);
            writer.WriteBE(MinVersion);
            writer.WriteBE(MaxVersion);
        }
    }
}
