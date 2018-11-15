using System;
using SmartGlass.Common;

namespace SmartGlass
{
    internal class MediaMetadata : ISerializable
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public void Deserialize(BEReader reader)
        {
            Name = reader.ReadUInt16PrefixedString();
            Value = reader.ReadUInt16PrefixedString();
        }

        public void Serialize(BEWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}
