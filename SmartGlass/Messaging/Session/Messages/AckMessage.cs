using System.Collections.Generic;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.Ack)]
    internal class AckMessage : SessionMessageBase
    {
        public uint LowWatermark { get; set; }
        public HashSet<uint> ProcessedList { get; set; }
        public HashSet<uint> RejectedList { get; set; }

        public override void Deserialize(BEReader reader)
        {
            LowWatermark = reader.ReadUInt32();
            ProcessedList = new HashSet<uint>(reader.ReadUInt32Array());
            RejectedList = new HashSet<uint>(reader.ReadUInt32Array());
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write(LowWatermark);
            writer.Write(ProcessedList);
            writer.Write(RejectedList);
        }
    }
}