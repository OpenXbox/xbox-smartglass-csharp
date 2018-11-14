using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [ChannelControlType(ChannelControlType.Open)]
    internal class ChannelOpen : ISerializableLE
    {
        public byte[] Flags { get; private set; }

        public ChannelOpen()
        {
        }

        public ChannelOpen(byte[] flags)
        {
            Flags = flags;
        }

        public void Deserialize(BinaryReader br)
        {
            Flags = br.ReadUInt32PrefixedBlob();
        }

        public void Serialize(BinaryWriter bw)
        {
            if (Flags != null && Flags.Length > 0)
            {
                bw.Write((uint)Flags.Length);
                bw.Write(Flags);
            }
            else
            {
                bw.Write((uint)0);
            }
        }
    }
}