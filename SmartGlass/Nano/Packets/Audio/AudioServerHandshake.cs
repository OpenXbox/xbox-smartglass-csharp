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

        internal override void DeserializeStreamer(BinaryReader reader)
        {
            ProtocolVersion = reader.ReadUInt32();
            ReferenceTimestamp = reader.ReadUInt64();
            Formats = reader.ReadUInt32PrefixedArray<AudioFormat>();
        }

        internal override void SerializeStreamer(BinaryWriter writer)
        {
            writer.Write(ProtocolVersion);
            writer.Write(ReferenceTimestamp);
            writer.WriteUInt32PrefixedArray(Formats);
        }
    }
}
