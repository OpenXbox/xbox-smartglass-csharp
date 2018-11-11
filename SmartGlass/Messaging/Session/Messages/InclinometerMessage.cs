using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.Inclinometer)]
    internal class InclinometerMessage : SessionMessageBase
    {
        public ulong Timestamp { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public float Yaw { get; set; }

        public override void Deserialize(BEReader reader)
        {
            Timestamp = reader.ReadUInt64();
            Pitch = reader.ReadInt32();
            Roll = reader.ReadInt32();
            Yaw = reader.ReadInt32();
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write(Timestamp);
            writer.Write(Pitch);
            writer.Write(Roll);
            writer.Write(Yaw);
        }
    }
}
