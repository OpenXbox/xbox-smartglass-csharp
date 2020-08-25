using System;
using SmartGlass.Channels.Broadcast.Messages;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SmartGlass.Channels {
    /// <summary>
    /// Broadcast message json converter.
    /// </summary>
    class JsonBaseMessageJsonConverter : JsonConverter<JsonBaseMessage>
    {
        public override JsonBaseMessage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                var result = new JsonBaseMessage
                {
                    msgid = document.RootElement.GetProperty("msgid").GetString(),
                    request = document.RootElement.GetProperty("request").GetString(),
                    @params = document.RootElement.GetProperty("@params").Clone()
                };
                return result;
            }
        }

        public override void Write(Utf8JsonWriter writer, JsonBaseMessage value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
