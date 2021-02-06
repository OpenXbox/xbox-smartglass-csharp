using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.Gyrometer)]
    internal record GyrometerMessage : SessionMessageBase
    {
        public ulong Timestamp { get; set; }
        public float AngularVelocityX { get; set; }
        public float AngularVelocityY { get; set; }
        public float AngularVelocityZ { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            Timestamp = reader.ReadUInt64BE();
            AngularVelocityX = reader.ReadInt32BE();
            AngularVelocityY = reader.ReadInt32BE();
            AngularVelocityZ = reader.ReadInt32BE();
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.WriteBE(Timestamp);
            writer.WriteBE(AngularVelocityX);
            writer.WriteBE(AngularVelocityY);
            writer.WriteBE(AngularVelocityZ);
        }
    }
}
