using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.Json)]
    internal class JsonMessage : SessionMessageBase
    {
        public JsonMessage()
        {
            Header.RequestAcknowledge = true;
        }

        public string Json { get; set; }

        public override void Deserialize(BEReader reader)
        {
            Json = reader.ReadUInt16PrefixedString();
        }

        public override void Serialize(BEWriter writer)
        {
            writer.WriteUInt16Prefixed(Json);
        }
    }
}