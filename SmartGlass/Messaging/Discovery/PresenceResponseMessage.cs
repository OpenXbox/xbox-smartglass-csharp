using System;
using SmartGlass.Connection;
using SmartGlass.Common;
using Org.BouncyCastle.X509;

namespace SmartGlass.Messaging.Discovery
{
    /// <summary>
    /// Presence response message.
    /// Sent from console to client as response to PresenceRequest.
    /// </summary>
    [MessageType(MessageType.PresenceResponse)]
    internal class PresenceResponseMessage : MessageBase<DiscoveryMessageHeader>
    {
        public DeviceFlags Flags { get; set; }
        public DeviceType DeviceType { get; set; }
        public string Name { get; set; }
        public Guid HardwareId { get; set; }
        public uint LastError { get; set; }
        public X509Certificate Certificate { get; set; }

        protected override void DeserializePayload(BEReader reader)
        {
            Flags = (DeviceFlags)reader.ReadUInt32();
            DeviceType = (DeviceType)reader.ReadUInt16();
            Name = reader.ReadUInt16PrefixedString();
            HardwareId = Guid.Parse(reader.ReadUInt16PrefixedString());
            LastError = reader.ReadUInt32();
            Certificate = CryptoExtensions
                            .DeserializeCertificateAsn(reader.ReadUInt16PrefixedBlob());
        }

        protected override void SerializePayload(BEWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
