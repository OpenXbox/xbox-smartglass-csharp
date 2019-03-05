using System.IO;
using SmartGlass.Common;

namespace SmartGlass.Nano.Packets
{
    public abstract class StreamerMessage : IStreamerMessage
    {
        public NanoChannel Channel { get; set; }
        public RtpHeader Header { get; set; }
        public StreamerHeader StreamerHeader { get; }
        public uint StreamerSize { get; set; }

        public StreamerMessage()
        {
            Header = new RtpHeader()
            {
                PayloadType = NanoPayloadType.Streamer
            };
            StreamerSize = 0;
            StreamerHeader = new StreamerHeader();
        }

        public StreamerMessage(uint StreamerType)
        {
            Header = new RtpHeader()
            {
                PayloadType = NanoPayloadType.Streamer
            };
            StreamerSize = 0;
            StreamerHeader = new StreamerHeader()
            {
                PacketType = StreamerType
            };
        }

        public void Deserialize(BinaryReader reader)
        {
            StreamerHeader.Deserialize(reader);
            StreamerSize = reader.ReadUInt32();
            DeserializeStreamer(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            // Write out payload first to get its size
            BinaryWriter tmpWriter = new BinaryWriter(new MemoryStream());
            SerializeStreamer(tmpWriter);
            byte[] streamerData = tmpWriter.ToBytes();
            StreamerSize = (uint)streamerData.Length;

            StreamerHeader.Serialize(writer);
            writer.Write(StreamerSize);
            writer.Write(streamerData);
        }

        internal abstract void DeserializeStreamer(BinaryReader reader);
        internal abstract void SerializeStreamer(BinaryWriter writer);
    }
}