using System;
using System.IO;
using DarkId.SmartGlass.Common;
using DarkId.SmartGlass.Nano;

namespace DarkId.SmartGlass.Nano.Packets
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

        public void Deserialize(LEReader br)
        {
            Flags = (AudioControlFlags)br.ReadUInt32();
        }

        public void Serialize(LEWriter bw)
        {
            bw.Write((uint)Flags);
        }
    }
}