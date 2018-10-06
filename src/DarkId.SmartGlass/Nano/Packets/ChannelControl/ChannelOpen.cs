using System;
using System.IO;
using DarkId.SmartGlass.Common;
using DarkId.SmartGlass.Nano;

namespace DarkId.SmartGlass.Nano.Packets
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

        public void Deserialize(LEReader br)
        {
            Flags = br.ReadBlobUInt32();
        }

        public void Serialize(LEWriter bw)
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