using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.Gyrometer)]
    internal class GyrometerMessage : SessionMessageBase
    {
        public ulong Timestamp { get; set; }
        public float AngularVelocityX { get; set; }
        public float AngularVelocityY { get; set; }
        public float AngularVelocityZ { get; set; }

        public override void Deserialize(BEReader reader)
        {
            Timestamp = reader.ReadUInt64();
            AngularVelocityX = reader.ReadInt32();
            AngularVelocityY = reader.ReadInt32();
            AngularVelocityZ = reader.ReadInt32();
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write(Timestamp);
            writer.Write(AngularVelocityX);
            writer.Write(AngularVelocityY);
            writer.Write(AngularVelocityZ);
        }
    }
}
