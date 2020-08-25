using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using SmartGlass.Channels.Broadcast.Messages;

namespace SmartGlass.Channels.Broadcast
{
    public class BroadcastMessageJsonConverter : JsonConverter<BroadcastBaseMessage>
    {
        public override BroadcastBaseMessage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                var messageType = (BroadcastMessageType)document.RootElement.GetProperty("type").GetInt32();

                switch (messageType)
                {
                    case BroadcastMessageType.GamestreamState:
                        var stateMessageType = (GamestreamStateMessageType)document.RootElement.GetProperty("state").GetInt32();
                        typeToConvert = GamestreamStateMessageTypeAttribute.GetTypeForMessageType(stateMessageType) ?? typeof(GamestreamStateBaseMessage);
                        break;
                    default:
                        typeToConvert = BroadcastMessageTypeAttribute.GetTypeForMessageType(messageType) ?? typeof(BroadcastBaseMessage);
                        break;

                }

                return (BroadcastBaseMessage)JsonSerializer.Deserialize(document.RootElement.GetRawText(), typeToConvert, options);
            }
        }

        public override void Write(Utf8JsonWriter writer, BroadcastBaseMessage value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
