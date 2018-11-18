using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [InputPayloadType(InputPayloadType.ClientHandshake)]
    public class InputClientHandshake : StreamerMessage
    {
        public uint MaxTouches { get; private set; }
        public ulong ReferenceTimestamp { get; private set; }

        public InputClientHandshake()
            : base((uint)InputPayloadType.ClientHandshake)
        {
        }

        public InputClientHandshake(uint maxTouches, ulong refTimestamp)
            : base((uint)InputPayloadType.ClientHandshake)
        {
            MaxTouches = maxTouches;
            ReferenceTimestamp = refTimestamp;
        }

        internal override void DeserializeStreamer(BinaryReader reader)
        {
            MaxTouches = reader.ReadUInt32();
            ReferenceTimestamp = reader.ReadUInt64();
        }

        internal override void SerializeStreamer(BinaryWriter writer)
        {
            writer.Write(MaxTouches);
            writer.Write(ReferenceTimestamp);
        }
    }
}