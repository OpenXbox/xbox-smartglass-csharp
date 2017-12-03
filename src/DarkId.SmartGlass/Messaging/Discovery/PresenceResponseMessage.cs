using System;
using DarkId.SmartGlass.Connection;
using DarkId.SmartGlass.Common;
using Org.BouncyCastle.X509;

namespace DarkId.SmartGlass.Messaging.Discovery
{
    [MessageType(MessageType.PresenceResponse)]
    internal class PresenceResponseMessage : MessageBase<DiscoveryMessageHeader>
    {
        public DeviceFlags Flags { get; set; }
        public DeviceType DeviceType { get; set; }
        public string Name { get; set; }
        public Guid HardwareId { get; set; }
        public X509Certificate Certificate { get; set; }

        protected override void DeserializePayload(BEReader reader)
        {
            reader.ReadBytes(2);

            Flags = (DeviceFlags)reader.ReadUInt16();
            DeviceType = (DeviceType)reader.ReadUInt16();
            Name = reader.ReadString();
            HardwareId = Guid.Parse(reader.ReadString());

            reader.ReadBytes(4);

            Certificate = CryptoExtensions.DeserializeCertificateAsn(reader.ReadBlob());
        }

        protected override void SerializePayload(BEWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}