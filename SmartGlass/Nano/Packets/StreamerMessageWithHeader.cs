using System.IO;

namespace SmartGlass.Nano.Packets
{
    public abstract class StreamerMessageWithHeader : IStreamerMessage
    {
        public NanoChannel Channel { get; set; }
        public RtpHeader Header { get; set; }
        public StreamerHeader StreamerHeader { get; }
        public StreamerControlHeader ControlHeader { get; set; }

        public StreamerMessageWithHeader()
        {
            Header = new RtpHeader()
            {
                PayloadType = NanoPayloadType.Streamer
            };
            StreamerHeader = new StreamerHeader()
            {
                PacketType = 0
            };
            ControlHeader = new StreamerControlHeader();
        }

        public StreamerMessageWithHeader(ControlOpCode opCode)
        {
            Header = new RtpHeader()
            {
                PayloadType = NanoPayloadType.Streamer
            };
            StreamerHeader = new StreamerHeader()
            {
                PacketType = 0
            };
            ControlHeader = new StreamerControlHeader(opCode);
        }

        public void Deserialize(BinaryReader reader)
        {
            StreamerHeader.Deserialize(reader);
            ControlHeader.Deserialize(reader);
            DeserializeStreamer(reader);
        }

        public void Serialize(BinaryWriter writer)
        {
            StreamerHeader.Serialize(writer);
            ControlHeader.Serialize(writer);
            SerializeStreamer(writer);
        }

        internal abstract void DeserializeStreamer(BinaryReader reader);
        internal abstract void SerializeStreamer(BinaryWriter writer);
    }
}