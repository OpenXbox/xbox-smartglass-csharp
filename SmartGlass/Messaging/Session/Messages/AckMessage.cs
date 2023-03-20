using System.Collections.Generic;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.Ack)]
    internal record AckMessage : SessionMessageBase
    {
        public uint LowWatermark { get; set; }
        public HashSet<uint> ProcessedList { get; set; }
        public HashSet<uint> RejectedList { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            LowWatermark = reader.ReadUInt32BE();
            ProcessedList = new HashSet<uint>(reader.ReadUInt32BEArray());
            RejectedList = new HashSet<uint>(reader.ReadUInt32BEArray());
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.WriteBE(LowWatermark);
            writer.WriteBE(ProcessedList);
            writer.WriteBE(RejectedList);
        }
    }
}