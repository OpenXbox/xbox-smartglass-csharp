using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.Accelerometer)]
    internal record AccelerometerMessage : SessionMessageBase
    {
        public ulong Timestamp { get; set; }
        public float AccelerationX { get; set; }
        public float AccelerationY { get; set; }
        public float AccelerationZ { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            Timestamp = reader.ReadUInt64BE();
            AccelerationX = reader.ReadInt32BE();
            AccelerationY = reader.ReadInt32BE();
            AccelerationZ = reader.ReadInt32BE();
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.WriteBE(Timestamp);
            writer.WriteBE(AccelerationX);
            writer.WriteBE(AccelerationY);
            writer.WriteBE(AccelerationZ);
        }
    }
}
