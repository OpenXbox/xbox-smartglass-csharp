using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [AudioPayloadType(AudioPayloadType.ServerHandshake)]
    public class AudioServerHandshake : StreamerMessage
    {
        public uint ProtocolVersion { get; private set; }
        public ulong ReferenceTimestamp { get; private set; }
        public AudioFormat[] Formats { get; private set; }

        public AudioServerHandshake()
            : base((uint)AudioPayloadType.ServerHandshake)
        {
        }

        public AudioServerHandshake(uint protocolVersion, ulong refTimestamp,
                                    AudioFormat[] formats)
            : base((uint)AudioPayloadType.ServerHandshake)
        {
            ProtocolVersion = protocolVersion;
            ReferenceTimestamp = refTimestamp;
            Formats = formats;
        }

        internal override void DeserializeStreamer(EndianReader reader)
        {
            ProtocolVersion = reader.ReadUInt32LE();
            ReferenceTimestamp = reader.ReadUInt64LE();
            Formats = reader.ReadUInt32LEPrefixedArray<AudioFormat>();
        }

        internal override void SerializeStreamer(EndianWriter writer)
        {
            writer.WriteLE(ProtocolVersion);
            writer.WriteLE(ReferenceTimestamp);
            writer.WriteUInt32LEPrefixedArray(Formats);
        }
    }
}
