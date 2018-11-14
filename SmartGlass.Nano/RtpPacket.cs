using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano
{
    internal class RtpPacket : ISerializable
    {
        public RtpHeader Header { get; private set; }
        public ISerializableLE Payload { get; private set; }

        private static ISerializableLE CreateFromPayloadType(RtpPayloadType payloadType)
        {
            var type = RtpPayloadTypeAttribute.GetTypeForMessageType(payloadType);
            if (type == null)
            {
                return null;
            }

            return (ISerializableLE)Activator.CreateInstance(type);
        }

        public static RtpPacket CreateFromBuffer(BEReader br)
        {
            var packet = new RtpPacket();
            packet.Deserialize(br);
            return packet;
        }

        public RtpPacket()
        {
            Header = new RtpHeader();
        }

        public RtpPacket(RtpHeader header)
        {
            Header = header;
        }

        public RtpPacket(RtpPayloadType payloadType, ISerializableLE payload)
        {
            Header = new RtpHeader();
            Header.PayloadType = payloadType;
            Payload = payload;
        }

        public void Deserialize(BEReader br)
        {
            Header.Deserialize(br);
            Payload = CreateFromPayloadType(Header.PayloadType);

            byte[] buf = br.ReadToEnd();
            BinaryReader payloadReader = BinaryExtensions.ReaderFromBytes(buf);
            Payload.Deserialize(payloadReader);
        }

        public void Serialize(BEWriter bw)
        {
            BinaryWriter payloadWriter = new BinaryWriter(new MemoryStream());
            Payload.Serialize(payloadWriter);
            byte[] padding = Padding.CreatePaddingData(
                PaddingType.ANSI_X923,
                payloadWriter.ToBytes(),
                alignment: 4);
            payloadWriter.Write(padding);

            if (padding.Length > 0)
            {
                Header.Padding = true;
            }

            Header.Serialize(bw);
            bw.Write(payloadWriter.ToBytes());
        }
    }
}
