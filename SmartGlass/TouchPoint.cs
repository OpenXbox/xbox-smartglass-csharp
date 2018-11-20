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

        void ISerializable.Deserialize(BEReader reader)
        {
            Id = reader.ReadUInt32();
            Action = (TouchAction)reader.ReadUInt16();
            PointX = reader.ReadUInt32();
            PointY = reader.ReadUInt32();
        }

        void ISerializable.Serialize(BEWriter writer)
        {
            writer.Write(Id);
            writer.Write((ushort)Action);
            writer.Write(PointX);
            writer.Write(PointY);
        }
    }
}
