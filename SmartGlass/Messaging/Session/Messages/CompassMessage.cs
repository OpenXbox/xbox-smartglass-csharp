using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.Compass)]
    internal record CompassMessage : SessionMessageBase
    {
        public ulong Timestamp { get; set; }
        public float MagneticNorth { get; set; }
        public float TrueNorth { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            Timestamp = reader.ReadUInt64BE();
            MagneticNorth = reader.ReadInt32BE();
            TrueNorth = reader.ReadInt32BE();
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.WriteBE(Timestamp);
            writer.WriteBE(MagneticNorth);
            writer.WriteBE(TrueNorth);
        }
    }
}
