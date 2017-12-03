using System;
using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass
{
    public class ActiveTitle : ISerializable
    {
        public uint TitleId { get; private set; }
        public ActiveTitleLocation TitleLocation { get; private set; }
        public bool HasFocus { get; private set; }
        public Guid ProductId { get; private set; }
        public Guid SandboxId { get; private set; }
        public string AumId { get; private set; }

        void ISerializable.Deserialize(BEReader reader)
        {
            TitleId = reader.ReadUInt32();

            // This is weird:
            HasFocus = reader.ReadByte() == 128;
            TitleLocation = (ActiveTitleLocation)reader.ReadByte();

            ProductId = new Guid(reader.ReadBytes(16));
            SandboxId = new Guid(reader.ReadBytes(16));

            AumId = reader.ReadString();
        }

        void ISerializable.Serialize(BEWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}