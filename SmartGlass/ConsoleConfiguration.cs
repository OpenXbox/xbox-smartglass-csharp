using System;
using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass
{
    public class ConsoleConfiguration : ISerializable
    {
        public uint LiveTVProvider { get; private set; }
        public uint MajorVersion { get; private set; }
        public uint MinorVersion { get; private set; }
        public uint BuildNumber { get; private set; }
        public string Locale { get; private set; }

        void ISerializable.Deserialize(BEReader reader)
        {
            LiveTVProvider = reader.ReadUInt32();
            MajorVersion = reader.ReadUInt32();
            MinorVersion = reader.ReadUInt32();
            BuildNumber = reader.ReadUInt32();
            Locale = reader.ReadString();
        }

        void ISerializable.Serialize(BEWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}