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

        internal override void DeserializeData(EndianReader reader)
        {
            Flags = reader.ReadUInt32LEPrefixedBlob();
        }

        internal override void SerializeData(EndianWriter writer)
        {
            if (Flags != null && Flags.Length > 0)
            {
                writer.WriteLE((uint)Flags.Length);
                writer.Write(Flags);
            }
            else
            {
                writer.WriteLE((uint)0);
            }
        }
    }
}