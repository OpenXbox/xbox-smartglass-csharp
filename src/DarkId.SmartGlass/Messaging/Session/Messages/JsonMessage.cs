using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Messaging.Session.Messages
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
            Json = reader.ReadString();
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write(Json);
        }
    }
}