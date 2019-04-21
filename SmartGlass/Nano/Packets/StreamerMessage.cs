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

        public void Deserialize(EndianReader reader)
        {
            StreamerHeader.Deserialize(reader);
            StreamerSize = reader.ReadUInt32LE();
            DeserializeStreamer(reader);
        }

        public void Serialize(EndianWriter writer)
        {
            // Write out payload first to get its size
            EndianWriter tmpWriter = new EndianWriter();
            SerializeStreamer(tmpWriter);
            byte[] streamerData = tmpWriter.ToBytes();
            StreamerSize = (uint)streamerData.Length;

            StreamerHeader.Serialize(writer);
            writer.WriteLE(StreamerSize);
            writer.Write(streamerData);
        }

        internal abstract void DeserializeStreamer(EndianReader reader);
        internal abstract void SerializeStreamer(EndianWriter writer);
    }
}