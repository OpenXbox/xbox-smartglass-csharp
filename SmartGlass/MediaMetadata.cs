using System;
using SmartGlass.Common;

namespace SmartGlass
{
    internal class MediaMetadata : ISerializable
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public void Deserialize(EndianReader reader)
        {
            Name = reader.ReadUInt16BEPrefixedString();
            Value = reader.ReadUInt16BEPrefixedString();
        }

        public void Serialize(EndianWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}
