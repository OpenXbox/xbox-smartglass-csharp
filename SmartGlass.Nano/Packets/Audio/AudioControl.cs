using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [AudioPayloadType(AudioPayloadType.Control)]
    public class AudioControl : StreamerMessage
    {
        public AudioControlFlags Flags { get; private set; }

        public AudioControl()
            : base((uint)AudioPayloadType.Control)
        {
        }

        public AudioControl(AudioControlFlags flags)
            : base((uint)AudioPayloadType.Control)
        {
            Flags = flags;
        }

        public override void DeserializeStreamer(BinaryReader reader)
        {
            Flags = (AudioControlFlags)reader.ReadUInt32();
        }

        public override void SerializeStreamer(BinaryWriter writer)
        {
            writer.Write((uint)Flags);
        }
    }
}