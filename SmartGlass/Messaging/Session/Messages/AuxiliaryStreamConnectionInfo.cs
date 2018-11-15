using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    internal class AuxiliaryStreamConnectionInfo : ISerializable
    {
        public byte[] CryptoKey { get; set; }
        public byte[] ServerInitVector { get; set; }
        public byte[] ClientInitVector { get; set; }
        public byte[] SignHash { get; set; }

        public AuxiliaryStreamEndpoint[] Endpoints { get; set; }

        public void Deserialize(BEReader reader)
        {
            CryptoKey = reader.ReadUInt16PrefixedBlob();
            ServerInitVector = reader.ReadUInt16PrefixedBlob();
            ClientInitVector = reader.ReadUInt16PrefixedBlob();
            SignHash = reader.ReadUInt16PrefixedBlob();

            Endpoints = reader.ReadUInt16PrefixedArray<AuxiliaryStreamEndpoint>();
        }

        public void Serialize(BEWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}