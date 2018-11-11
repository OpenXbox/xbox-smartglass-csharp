using System;
using System.IO;
using DarkId.SmartGlass.Common;
using DarkId.SmartGlass.Nano;

namespace DarkId.SmartGlass.Nano.Packets
{
    [ChannelControlType(ChannelControlType.Close)]
    internal class ChannelClose : ISerializableLE
    {
        public uint Flags { get; private set; }

        public ChannelClose()
        {
        }
        
        public ChannelClose(uint flags)
        {
            Flags = flags;
        }

        public void Deserialize(LEReader br)
        {
            Flags = br.ReadUInt32();
        }

        public void Serialize(LEWriter bw)
        {
            bw.Write(Flags);
        }
    }
}