using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    internal record AuxiliaryStreamEndpoint : ISerializable
    {
        public string Host { get; set; }
        public string Service { get; set; }

        public void Deserialize(EndianReader reader)
        {
            Host = reader.ReadUInt16BEPrefixedString();
            Service = reader.ReadUInt16BEPrefixedString();
        }

        public void Serialize(EndianWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}