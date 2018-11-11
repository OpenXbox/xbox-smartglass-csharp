using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.Orientation)]
    internal class OrientationMessage : SessionMessageBase
    {
        public ulong Timestamp { get; set; }
        public float RotationMatrixValue { get; set; }
        public float RotationW { get; set; }
        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }

        public override void Deserialize(BEReader reader)
        {
            Timestamp = reader.ReadUInt64();
            RotationMatrixValue = reader.ReadInt32();
            RotationW = reader.ReadInt32();
            RotationX = reader.ReadInt32();
            RotationY = reader.ReadInt32();
            RotationZ = reader.ReadInt32();
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write(Timestamp);
            writer.Write(RotationMatrixValue);
            writer.Write(RotationW);
            writer.Write(RotationX);
            writer.Write(RotationY);
            writer.Write(RotationZ);
        }
    }
}
