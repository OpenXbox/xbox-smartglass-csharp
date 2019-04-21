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

        protected override void DeserializePayload(EndianReader reader)
        {
            Flags = (DeviceFlags)reader.ReadUInt32BE();
            DeviceType = (DeviceType)reader.ReadUInt16BE();
            Name = reader.ReadUInt16BEPrefixedString();
            HardwareId = Guid.Parse(reader.ReadUInt16BEPrefixedString());
            LastError = reader.ReadUInt32BE();
            Certificate = CryptoExtensions
                            .DeserializeCertificateAsn(reader.ReadUInt16BEPrefixedBlob());
        }

        protected override void SerializePayload(EndianWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
