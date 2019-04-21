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

        public override void Deserialize(EndianReader reader)
        {
            Json = reader.ReadUInt16BEPrefixedString();
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.WriteUInt16BEPrefixed(Json);
        }
    }
}