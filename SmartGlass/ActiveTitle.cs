using System;
using SmartGlass.Common;

namespace SmartGlass
{
    public class ActiveTitle : ISerializable
    {
        public uint TitleId { get; private set; }
        public ActiveTitleLocation TitleLocation { get; private set; }
        public bool HasFocus { get; private set; }
        public Guid ProductId { get; private set; }
        public Guid SandboxId { get; private set; }
        public string AumId { get; private set; }

        void ISerializable.Deserialize(EndianReader reader)
        {
            TitleId = reader.ReadUInt32BE();

            ushort titleDisposition = reader.ReadUInt16BE();
            HasFocus = (titleDisposition & 0x8000) == 0x8000;
            TitleLocation = (ActiveTitleLocation)(titleDisposition & 0x7FFF);

            ProductId = new Guid(reader.ReadBytes(16));
            SandboxId = new Guid(reader.ReadBytes(16));

            AumId = reader.ReadUInt16BEPrefixedString();
        }

        void ISerializable.Serialize(EndianWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}