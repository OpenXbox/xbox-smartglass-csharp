using System;
using SmartGlass.Common;

namespace SmartGlass
{
    internal class TextDelta : ISerializable
    {
        public uint Offset { get; set; }
        public uint DeleteCount { get; set; }
        public string InsertContent { get; set; }

        public void Deserialize(EndianReader reader)
        {
            Offset = reader.ReadUInt32BE();
            DeleteCount = reader.ReadUInt32BE();
            InsertContent = reader.ReadUInt16BEPrefixedString();
        }

        public void Serialize(EndianWriter writer)
        {
            writer.WriteBE(Offset);
            writer.WriteBE(DeleteCount);
            writer.WriteUInt16BEPrefixed(InsertContent);
        }
    }
}
