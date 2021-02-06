using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    internal record AuxiliaryStreamConnectionInfo : ISerializable
    {
        public byte[] CryptoKey { get; set; }
        public byte[] ServerInitVector { get; set; }
        public byte[] ClientInitVector { get; set; }
        public byte[] SignHash { get; set; }

        public AuxiliaryStreamEndpoint[] Endpoints { get; set; }

        public void Deserialize(EndianReader reader)
        {
            CryptoKey = reader.ReadUInt16BEPrefixedBlob();
            ServerInitVector = reader.ReadUInt16BEPrefixedBlob();
            ClientInitVector = reader.ReadUInt16BEPrefixedBlob();
            SignHash = reader.ReadUInt16BEPrefixedBlob();

            Endpoints = reader.ReadUInt16BEPrefixedArray<AuxiliaryStreamEndpoint>();
        }

        public void Serialize(EndianWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}