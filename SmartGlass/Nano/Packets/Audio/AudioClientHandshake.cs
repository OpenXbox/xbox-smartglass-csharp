using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [AudioPayloadType(AudioPayloadType.ClientHandshake)]
    public class AudioClientHandshake : StreamerMessage
    {
        public uint InitialFrameID { get; private set; }
        public AudioFormat RequestedFormat { get; private set; }

        public AudioClientHandshake()
            : base((uint)AudioPayloadType.ClientHandshake)
        {
            RequestedFormat = new AudioFormat();
        }

        public AudioClientHandshake(uint initialFrameID, AudioFormat requestedFormat)
            : base((uint)AudioPayloadType.ClientHandshake)
        {
            InitialFrameID = initialFrameID;
            RequestedFormat = requestedFormat;
        }

        internal override void DeserializeStreamer(EndianReader reader)
        {
            InitialFrameID = reader.ReadUInt32LE();
            ((ISerializable)RequestedFormat).Deserialize(reader);
        }

        internal override void SerializeStreamer(EndianWriter writer)
        {
            writer.WriteLE(InitialFrameID);
            ((ISerializable)RequestedFormat).Serialize(writer);
        }
    }
}
