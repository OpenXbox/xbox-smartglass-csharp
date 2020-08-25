using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartGlass.Json
{
    public class GuidConverter : JsonConverter<Guid>
    {
        public override Guid Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return Guid.Empty;

                case JsonTokenType.String:
                    var str = reader.GetString();
                    return string.IsNullOrWhiteSpace(str) ? Guid.Empty : new Guid(str);

                default:
                    throw new ArgumentException("Invalid token type");
            }
        }

        public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value == Guid.Empty ? "" : value.ToString());
        }
    }
}