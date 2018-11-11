using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.Accelerometer)]
    internal class AccelerometerMessage : SessionMessageBase
    {
        public ulong Timestamp { get; set; }
        public float AccelerationX { get; set; }
        public float AccelerationY { get; set; }
        public float AccelerationZ { get; set; }

        public override void Deserialize(BEReader reader)
        {
            Timestamp = reader.ReadUInt64();
            AccelerationX = reader.ReadInt32();
            AccelerationY = reader.ReadInt32();
            AccelerationZ = reader.ReadInt32();
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write(Timestamp);
            writer.Write(AccelerationX);
            writer.Write(AccelerationY);
            writer.Write(AccelerationZ);
        }
    }
}
