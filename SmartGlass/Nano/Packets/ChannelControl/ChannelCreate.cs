using System;
using System.IO;
using System.Text;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [ChannelControlType(ChannelControlType.Create)]
    public class ChannelCreate : ChannelControlMessage
    {
        public string Name { get; private set; }
        public uint Flags { get; private set; }

        public ChannelCreate()
            : base(ChannelControlType.Create)
        {
        }

        public ChannelCreate(string name, uint flags)
            : base(ChannelControlType.Create)
        {
            Name = name;
            Flags = flags;
        }

        internal override void DeserializeData(EndianReader reader)
        {
            byte[] name = reader.ReadUInt16LEPrefixedBlob();
            Name = Encoding.GetEncoding("utf-8").GetString(name);
            Flags = reader.ReadUInt32LE();
        }

        internal override void SerializeData(EndianWriter writer)
        {
            byte[] name = Encoding.GetEncoding("utf-8").GetBytes(Name);
            writer.WriteUInt16LEPrefixedBlob(name);
            writer.WriteLE(Flags);
        }
    }
}