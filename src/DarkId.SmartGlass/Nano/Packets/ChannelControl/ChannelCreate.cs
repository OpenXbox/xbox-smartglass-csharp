using System;
using System.IO;
using System.Text;
using DarkId.SmartGlass.Common;
using DarkId.SmartGlass.Nano;

namespace DarkId.SmartGlass.Nano.Packets
{
    [ChannelControlType(ChannelControlType.Create)]
    internal class ChannelCreate : ISerializableLE
    {
        public string Name { get; private set; }
        public uint Flags { get; private set; }

        public ChannelCreate()
        {
        }
        
        public ChannelCreate(string name, uint flags)
        {
            Name = name;
            Flags = flags;
        }

        public void Deserialize(LEReader br)
        {
            byte[] name = br.ReadBlobUInt16();
            Name = Encoding.GetEncoding("utf-8").GetString(name);
            Flags = br.ReadUInt32();
        }

        public void Serialize(LEWriter bw)
        {
            byte[] name = Encoding.GetEncoding("utf-8").GetBytes(Name);
            bw.Write((ushort)name.Length);
            bw.Write(name);
            bw.Write(Flags);
        }
    }
}