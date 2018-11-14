using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [VideoPayloadType(VideoPayloadType.ClientHandshake)]
    internal class VideoClientHandshake : ISerializableLE
    {
        public uint InitialFrameId { get; private set; }
        public VideoFormat RequestedFormat { get; private set; }

        public VideoClientHandshake()
        {
            RequestedFormat = new VideoFormat();
        }

        public VideoClientHandshake(uint initialFrameId, VideoFormat requestedFormat)
        {
            InitialFrameId = initialFrameId;
            RequestedFormat = requestedFormat;
        }

        public void Deserialize(BinaryReader br)
        {
            InitialFrameId = br.ReadUInt32();
            ((ISerializableLE)RequestedFormat).Deserialize(br);
        }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write(InitialFrameId);
            ((ISerializableLE)RequestedFormat).Serialize(bw);
        }
    }
}
