using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.Inclinometer)]
    internal record InclinometerMessage : SessionMessageBase
    {
        public ulong Timestamp { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public float Yaw { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            Timestamp = reader.ReadUInt64BE();
            Pitch = reader.ReadInt32BE();
            Roll = reader.ReadInt32BE();
            Yaw = reader.ReadInt32BE();
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.WriteBE(Timestamp);
            writer.WriteBE(Pitch);
            writer.WriteBE(Roll);
            writer.WriteBE(Yaw);
        }
    }
}
