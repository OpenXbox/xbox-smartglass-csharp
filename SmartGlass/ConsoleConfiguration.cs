using System;
using SmartGlass.Common;

namespace SmartGlass
{
    public record ConsoleConfiguration : ISerializable
    {
        public uint LiveTVProvider { get; private set; }
        public uint MajorVersion { get; private set; }
        public uint MinorVersion { get; private set; }
        public uint BuildNumber { get; private set; }
        public string Locale { get; private set; }

        void ISerializable.Deserialize(EndianReader reader)
        {
            LiveTVProvider = reader.ReadUInt32BE();
            MajorVersion = reader.ReadUInt32BE();
            MinorVersion = reader.ReadUInt32BE();
            BuildNumber = reader.ReadUInt32BE();
            Locale = reader.ReadUInt16BEPrefixedString();
        }

        void ISerializable.Serialize(EndianWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}