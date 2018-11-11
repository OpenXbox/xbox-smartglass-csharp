using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [InputPayloadType(InputPayloadType.ClientHandshake)]
    internal class InputClientHandshake : ISerializableLE
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

        public void Deserialize(LEReader br)
        {
            MaxTouches = br.ReadUInt32();
            ReferenceTimestamp = br.ReadUInt64();
        }

        public void Serialize(LEWriter bw)
        {
            bw.Write(MaxTouches);
            bw.Write(ReferenceTimestamp);
        }
    }
}