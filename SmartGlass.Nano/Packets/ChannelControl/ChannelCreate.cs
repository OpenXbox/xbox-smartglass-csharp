using System;
using System.IO;
using System.Text;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
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

        public void Deserialize(BinaryReader br)
        {
            byte[] name = br.ReadUInt16PrefixedBlob();
            Name = Encoding.GetEncoding("utf-8").GetString(name);
            Flags = br.ReadUInt32();
        }

        public void Serialize(BinaryWriter bw)
        {
            byte[] name = Encoding.GetEncoding("utf-8").GetBytes(Name);
            bw.Write((ushort)name.Length);
            bw.Write(name);
            bw.Write(Flags);
        }
    }
}