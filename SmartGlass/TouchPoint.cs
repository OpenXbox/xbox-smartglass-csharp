using System;
using SmartGlass.Common;

namespace SmartGlass
{
    public class TouchPoint : ISerializable
    {
        public uint Id { get; set; }
        public TouchAction Action { get; set; }
        public uint PointX { get; set; }
        public uint PointY { get; set; }

        void ISerializable.Deserialize(EndianReader reader)
        {
            Id = reader.ReadUInt32BE();
            Action = (TouchAction)reader.ReadUInt16BE();
            PointX = reader.ReadUInt32BE();
            PointY = reader.ReadUInt32BE();
        }

        void ISerializable.Serialize(EndianWriter writer)
        {
            writer.WriteBE(Id);
            writer.WriteBE((ushort)Action);
            writer.WriteBE(PointX);
            writer.WriteBE(PointY);
        }
    }
}
