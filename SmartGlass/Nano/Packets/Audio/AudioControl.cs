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

        internal override void DeserializeStreamer(EndianReader reader)
        {
            Flags = (AudioControlFlags)reader.ReadUInt32LE();
        }

        internal override void SerializeStreamer(EndianWriter writer)
        {
            writer.WriteLE((uint)Flags);
        }
    }
}