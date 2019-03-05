using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [VideoPayloadType(VideoPayloadType.ClientHandshake)]
    public class VideoClientHandshake : StreamerMessage
    {
        public uint InitialFrameId { get; private set; }
        public VideoFormat RequestedFormat { get; private set; }

        public VideoClientHandshake()
            : base((uint)VideoPayloadType.ClientHandshake)
        {
            RequestedFormat = new VideoFormat();
        }

        public VideoClientHandshake(uint initialFrameId, VideoFormat requestedFormat)
            : base((uint)VideoPayloadType.ClientHandshake)
        {
            InitialFrameId = initialFrameId;
            RequestedFormat = requestedFormat;
        }

        internal override void DeserializeStreamer(BinaryReader reader)
        {
            InitialFrameId = reader.ReadUInt32();
            ((ISerializableLE)RequestedFormat).Deserialize(reader);
        }

        internal override void SerializeStreamer(BinaryWriter writer)
        {
            writer.Write(InitialFrameId);
            ((ISerializableLE)RequestedFormat).Serialize(writer);
        }
    }
}
