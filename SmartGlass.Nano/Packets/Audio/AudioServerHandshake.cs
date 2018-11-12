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

        public void Deserialize(LEReader br)
        {
            ProtocolVersion = br.ReadUInt32();
            ReferenceTimestamp = br.ReadUInt64();
            Formats = br.ReadArrayUInt32<AudioFormat>();
        }

        public void Serialize(LEWriter bw)
        {
            bw.Write(ProtocolVersion);
            bw.Write(ReferenceTimestamp);
            bw.Write((uint)Formats.Length);
            foreach (ISerializableLE f in Formats)
            {
                f.Serialize(bw);
            }
        }
    }
}
