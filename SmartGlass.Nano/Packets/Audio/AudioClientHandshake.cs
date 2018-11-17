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

        public override void DeserializeStreamer(BinaryReader reader)
        {
            InitialFrameID = reader.ReadUInt32();
            ((ISerializableLE)RequestedFormat).Deserialize(reader);
        }

        public override void SerializeStreamer(BinaryWriter writer)
        {
            writer.Write(InitialFrameID);
            ((ISerializableLE)RequestedFormat).Serialize(writer);
        }
    }
}
