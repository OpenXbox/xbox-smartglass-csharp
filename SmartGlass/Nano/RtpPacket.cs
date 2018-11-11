using System;
using System.IO;
using DarkId.SmartGlass.Common;
using DarkId.SmartGlass.Nano;
using DarkId.SmartGlass.Nano.Packets;

namespace DarkId.SmartGlass.Nano
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

        public RtpPacket(RtpPayloadType payloadType)
        {
            Header = new RtpHeader();
            Header.PayloadType = payloadType;
        }
        
        public RtpPacket(RtpHeader header)
        {
            Header = header;
        }

        public void SetPayload(ISerializableLE payload)
        {
            Payload = payload;
        }

        public void Deserialize(BEReader br)
        {
            Header.Deserialize(br);
            Payload = CreateFromPayloadType(Header.PayloadType);

            byte[] buf = br.ReadToEnd();
            LEReader payloadReader = new LEReader(buf);
            Payload.Deserialize(payloadReader);
        }

        public void Serialize(BEWriter bw)
        {
            LEWriter payloadWriter = new LEWriter();
            Payload.Serialize(payloadWriter);

            long payloadLength = payloadWriter.BaseStream.Length;
            if (payloadLength % 4 != 0)
            {
                // ANSI X.923 padding, 4 byte alignment
                byte[] padding = new byte[4 - (payloadLength % 4)];
                padding[padding.Length - 1] = (byte)padding.Length;
                payloadWriter.Write(padding);
                Header.Padding = true;
            }

            Header.Serialize(bw);
            bw.Write(payloadWriter.ToBytes());
        }
    }
}