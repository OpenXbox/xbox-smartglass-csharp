using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.Orientation)]
    internal record OrientationMessage : SessionMessageBase
    {
        public ulong Timestamp { get; set; }
        public float RotationMatrixValue { get; set; }
        public float RotationW { get; set; }
        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            Timestamp = reader.ReadUInt64BE();
            RotationMatrixValue = reader.ReadInt32BE();
            RotationW = reader.ReadInt32BE();
            RotationX = reader.ReadInt32BE();
            RotationY = reader.ReadInt32BE();
            RotationZ = reader.ReadInt32BE();
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.WriteBE(Timestamp);
            writer.WriteBE(RotationMatrixValue);
            writer.WriteBE(RotationW);
            writer.WriteBE(RotationX);
            writer.WriteBE(RotationY);
            writer.WriteBE(RotationZ);
        }
    }
}
