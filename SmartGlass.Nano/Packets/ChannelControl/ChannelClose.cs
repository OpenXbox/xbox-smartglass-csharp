using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
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

        public void Deserialize(BinaryReader br)
        {
            Flags = br.ReadUInt32();
        }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write(Flags);
        }
    }
}