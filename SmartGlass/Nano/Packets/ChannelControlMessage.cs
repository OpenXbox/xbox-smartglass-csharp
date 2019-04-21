using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [NanoPayloadType(NanoPayloadType.ChannelControl)]
    public abstract class ChannelControlMessage : INanoPacket
    {
        public NanoChannel Channel { get; set; }
        public RtpHeader Header { get; set; }
        public ChannelControlType Type { get; set; }

        public ChannelControlMessage()
        {
            Header = new RtpHeader()
            {
                PayloadType = NanoPayloadType.ChannelControl
            };
        }

        public ChannelControlMessage(ChannelControlType type)
        {
            Header = new RtpHeader()
            {
                PayloadType = NanoPayloadType.ChannelControl
            };
            Type = type;
        }

        public void Deserialize(EndianReader reader)
        {
            Type = (ChannelControlType)reader.ReadUInt32LE();
            DeserializeData(reader);
        }

        public void Serialize(EndianWriter writer)
        {
            writer.WriteLE((uint)Type);
            SerializeData(writer);
        }

        internal abstract void DeserializeData(EndianReader reader);
        internal abstract void SerializeData(EndianWriter writer);
    }
}