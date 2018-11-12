using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [RtpPayloadType(RtpPayloadType.Streamer)]
    internal class Streamer : ISerializableLE
    {
        public StreamerFlags Flags { get; internal set; }
        public uint SequenceNumber { get; internal set; }
        public uint PreviousSequenceNumber { get; internal set; }
        public uint PacketType { get; internal set; }
        public uint PacketSize { get; internal set; }

        private byte[] RawData;
        public ISerializableLE Data { get; internal set; }

        public Streamer()
        {
        }

        public Streamer(uint packetType)
        {
            PacketType = packetType;
        }

        public void Deserialize(LEReader br)
        {
            Flags = (StreamerFlags)br.ReadUInt32();
            if (Flags.HasFlag(StreamerFlags.GotSeqAndPrev))
            {
                SequenceNumber = br.ReadUInt32();
                PreviousSequenceNumber = br.ReadUInt32();
            }
            PacketType = br.ReadUInt32();
            if (PacketType != 0)
            {
                PacketSize = br.ReadUInt32();
            }

            RawData = br.ReadToEnd();
        }

        public void Serialize(LEWriter bw)
        {
            var payloadWriter = new LEWriter();
            Data.Serialize(payloadWriter);
            var payload = payloadWriter.ToBytes();
            PacketSize = (uint)payload.Length;

            bw.Write((uint)Flags);
            if (Flags.HasFlag(StreamerFlags.GotSeqAndPrev))
            {
                bw.Write(SequenceNumber);
                bw.Write(PreviousSequenceNumber);
            }
            bw.Write(PacketType);
            if (PacketType != 0)
            {
                bw.Write(PacketSize);
            }
            bw.Write(payload);
        }

        public void DeserializeData(ISerializableLE payloadType)
        {
            Data = payloadType;
            Data.Deserialize(new LEReader(RawData));
        }
    }
}