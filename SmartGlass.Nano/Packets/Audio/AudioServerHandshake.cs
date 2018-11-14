using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [AudioPayloadType(AudioPayloadType.ServerHandshake)]
    internal class AudioServerHandshake : ISerializableLE
    {
        public uint ProtocolVersion { get; private set; }
        public ulong ReferenceTimestamp { get; private set; }
        public AudioFormat[] Formats { get; private set; }

        public AudioServerHandshake()
        {
        }

        public AudioServerHandshake(uint protocolVersion, ulong refTimestamp,
                                    AudioFormat[] formats)
        {
            ProtocolVersion = protocolVersion;
            ReferenceTimestamp = refTimestamp;
            Formats = formats;
        }

        public void Deserialize(BinaryReader br)
        {
            ProtocolVersion = br.ReadUInt32();
            ReferenceTimestamp = br.ReadUInt64();
            Formats = br.ReadUInt32PrefixedArray<AudioFormat>();
        }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write(ProtocolVersion);
            bw.Write(ReferenceTimestamp);
            bw.WriteUInt32PrefixedArray(Formats);
        }
    }
}
