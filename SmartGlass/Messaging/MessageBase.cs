using System;
using System.Net;
using System.Text.Json.Serialization;
using SmartGlass.Common;

namespace SmartGlass.Messaging
{
    /// <summary>
    /// Message base.
    /// </summary>
    abstract class MessageBase<THeader> : IMessage<THeader>
        where THeader : IMessageHeader, new()
    {
        public string TypeName => GetType().Name;

        public THeader Header { get; set; }

        IMessageHeader IMessage.Header => Header;

        [JsonIgnore]
        public IPEndPoint Origin { get; set; }

        public DateTime ClientReceivedTimestamp { get; set; }

        public MessageBase()
        {
            Header = new THeader()
            {
                Type = MessageTypeAttribute.GetMessageTypeForType(GetType())
            };
        }

        protected abstract void SerializePayload(EndianWriter writer);

        protected abstract void DeserializePayload(EndianReader reader);

        public virtual void Serialize(EndianWriter writer)
        {
            var payloadWriter = new EndianWriter();
            SerializePayload(payloadWriter);

            var payload = payloadWriter.ToBytes();
            Header.PayloadLength = (ushort)payload.Length;

            Header.Serialize(writer);
            writer.Write(payload);
        }

        public virtual void Deserialize(EndianReader reader)
        {
            Header.Deserialize(reader);
            DeserializePayload(reader.CreateChild(Header.PayloadLength));
        }
    }
}
