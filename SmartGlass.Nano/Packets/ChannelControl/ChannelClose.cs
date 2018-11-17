using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [ChannelControlType(ChannelControlType.Close)]
    public class ChannelClose : ChannelControlMessage
    {
        public uint Flags { get; private set; }

        public ChannelClose()
            : base(ChannelControlType.Close)
        {
        }

        public ChannelClose(uint flags)
            : base(ChannelControlType.Close)
        {
            Flags = flags;
        }

        public override void DeserializeData(BinaryReader reader)
        {
            Flags = reader.ReadUInt32();
        }

        public override void SerializeData(BinaryWriter writer)
        {
            writer.Write(Flags);
        }
    }
}