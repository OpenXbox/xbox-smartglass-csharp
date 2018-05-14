using System;
using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Messaging.Session.Messages
{
    internal class TouchMessage : SessionMessageBase
    {
        public uint Timestamp { get; set; }
        public TouchPoint[] Touchpoints { get; set; }

        public override void Deserialize(BEReader reader)
        {
            Timestamp = reader.ReadUInt32();
            Touchpoints = reader.ReadArray<TouchPoint>();
        }

        public override void Serialize(BEWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}