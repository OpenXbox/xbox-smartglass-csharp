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

        internal override void DeserializeStreamer(EndianReader reader)
        {
            InitialFrameId = reader.ReadUInt32LE();
            ((ISerializable)RequestedFormat).Deserialize(reader);
        }

        internal override void SerializeStreamer(EndianWriter writer)
        {
            writer.WriteLE(InitialFrameId);
            ((ISerializable)RequestedFormat).Serialize(writer);
        }
    }
}
