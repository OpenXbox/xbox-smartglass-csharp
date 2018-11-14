using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [AudioPayloadType(AudioPayloadType.Control)]
    internal class AudioControl : ISerializableLE
    {
        public AudioControlFlags Flags { get; private set; }

        public AudioControl()
        {
        }
        
        public AudioControl(AudioControlFlags flags)
        {
            Flags = flags;
        }

        public void Deserialize(BinaryReader br)
        {
            Flags = (AudioControlFlags)br.ReadUInt32();
        }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write((uint)Flags);
        }
    }
}