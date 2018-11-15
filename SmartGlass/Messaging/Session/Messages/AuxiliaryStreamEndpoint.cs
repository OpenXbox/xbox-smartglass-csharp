using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    internal class AuxiliaryStreamEndpoint : ISerializable
    {
        public string Host { get; set; }
        public string Service { get; set; }

        public void Deserialize(BEReader reader)
        {
            Host = reader.ReadUInt16PrefixedString();
            Service = reader.ReadUInt16PrefixedString();
        }

        public void Serialize(BEWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}