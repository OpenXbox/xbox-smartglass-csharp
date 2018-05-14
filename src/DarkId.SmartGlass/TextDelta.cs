using System;
using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass
{
    internal class TextDelta : ISerializable
    {
        public uint Offset { get; set; }
        public uint DeleteCount { get; set; }
        public string InsertContent { get; set; }

        public void Deserialize(BEReader reader)
        {
            Offset = reader.ReadUInt32();
            DeleteCount = reader.ReadUInt32();
            InsertContent = reader.ReadString();
        }

        public void Serialize(BEWriter writer)
        {
            writer.Write(Offset);
            writer.Write(DeleteCount);
            writer.Write(InsertContent);
        }
    }
}
