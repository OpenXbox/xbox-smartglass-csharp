using System;
using System.Net;
using SmartGlass.Common;
using Newtonsoft.Json;

namespace SmartGlass.Messaging
{
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

        protected abstract void SerializePayload(BEWriter writer);

        protected abstract void DeserializePayload(BEReader reader);

        public virtual void Serialize(BEWriter writer)
        {
            var payloadWriter = new BEWriter();
            SerializePayload(payloadWriter);

            var payload = payloadWriter.ToArray();
            Header.PayloadLength = (ushort)payload.Length;

            Header.Serialize(writer);
            writer.Write(payload);
        }

        public virtual void Deserialize(BEReader reader)
        {
            Header.Deserialize(reader);
            DeserializePayload(reader.CreateChild(Header.PayloadLength));
        }
    }
}