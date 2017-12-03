using System;
using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Messaging.Session.Messages
{
    internal class AuxiliaryStreamEndpoint : ISerializable
    {
        public string Host { get; set; }
        public string Service { get; set; }

        public void Deserialize(BEReader reader)
        {
            Host = reader.ReadString();
            Service = reader.ReadString();
        }

        public void Serialize(BEWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}