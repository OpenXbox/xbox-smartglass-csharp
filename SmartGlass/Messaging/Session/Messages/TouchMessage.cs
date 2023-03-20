using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    internal record TouchMessage : SessionMessageBase
    {
        public uint Timestamp { get; set; }
        public TouchPoint[] Touchpoints { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            Timestamp = reader.ReadUInt32BE();
            Touchpoints = reader.ReadUInt16BEPrefixedArray<TouchPoint>();
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.WriteBE(Timestamp);
            writer.WriteBE((ushort)Touchpoints.Length);
            foreach (TouchPoint p in Touchpoints)
                ((ISerializable)p).Serialize(writer);
        }
    }
}