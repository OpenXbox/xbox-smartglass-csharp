using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [ChannelControlType(ChannelControlType.Open)]
    public class ChannelOpen : ChannelControlMessage
    {
        public byte[] Flags { get; private set; }

        public ChannelOpen()
            : base(ChannelControlType.Open)
        {
            Flags = new byte[0];
        }

        public ChannelOpen(byte[] flags)
            : base(ChannelControlType.Open)
        {
            Flags = flags;
        }

        public override void DeserializeData(BinaryReader reader)
        {
            Flags = reader.ReadUInt32PrefixedBlob();
        }

        public override void SerializeData(BinaryWriter writer)
        {
            if (Flags != null && Flags.Length > 0)
            {
                writer.Write((uint)Flags.Length);
                writer.Write(Flags);
            }
            else
            {
                writer.Write((uint)0);
            }
        }
    }
}