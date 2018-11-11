using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.Compass)]
    internal class CompassMessage : SessionMessageBase
    {
        public ulong Timestamp { get; set; }
        public float MagneticNorth { get; set; }
        public float TrueNorth { get; set; }

        public override void Deserialize(BEReader reader)
        {
            Timestamp = reader.ReadUInt64();
            MagneticNorth = reader.ReadInt32();
            TrueNorth = reader.ReadInt32();
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write(Timestamp);
            writer.Write(MagneticNorth);
            writer.Write(TrueNorth);
        }
    }
}
