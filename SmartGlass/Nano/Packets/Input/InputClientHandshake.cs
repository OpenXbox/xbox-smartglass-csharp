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

        internal override void DeserializeStreamer(EndianReader reader)
        {
            MaxTouches = reader.ReadUInt32LE();
            ReferenceTimestamp = reader.ReadUInt64LE();
        }

        internal override void SerializeStreamer(EndianWriter writer)
        {
            writer.WriteLE(MaxTouches);
            writer.WriteLE(ReferenceTimestamp);
        }
    }
}