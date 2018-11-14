using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [InputPayloadType(InputPayloadType.ClientHandshake)]
    public class InputClientHandshake : ISerializableLE
    {
        public uint MaxTouches { get; private set; }
        public ulong ReferenceTimestamp { get; private set; }

        public InputClientHandshake()
        {
        }

        public InputClientHandshake(uint maxTouches, ulong refTimestamp)
        {
            MaxTouches = maxTouches;
            ReferenceTimestamp = refTimestamp;
        }

        void ISerializableLE.Deserialize(BinaryReader br)
        {
            MaxTouches = br.ReadUInt32();
            ReferenceTimestamp = br.ReadUInt64();
        }

        void ISerializableLE.Serialize(BinaryWriter bw)
        {
            bw.Write(MaxTouches);
            bw.Write(ReferenceTimestamp);
        }
    }
}