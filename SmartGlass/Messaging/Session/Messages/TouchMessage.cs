using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    internal class TouchMessage : SessionMessageBase
    {
        public uint Timestamp { get; set; }
        public TouchPoint[] Touchpoints { get; set; }

        public override void Deserialize(BEReader reader)
        {
            Timestamp = reader.ReadUInt32();
            Touchpoints = reader.ReadUInt16PrefixedArray<TouchPoint>();
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write(Timestamp);
            writer.Write((ushort)Touchpoints.Length);
            foreach (TouchPoint p in Touchpoints)
                ((ISerializable)p).Serialize(writer);
        }
    }
}