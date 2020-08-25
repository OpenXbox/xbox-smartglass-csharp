using System;
using System.Buffers;
using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartGlass.Json
{
    public class BooleanConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
                if (Utf8Parser.TryParse(span, out bool value, out int bytesConsumed) && span.Length == bytesConsumed)
                    return value;

                if (Boolean.TryParse(reader.GetString(), out value))
                    return value;
            }

            return reader.GetBoolean();
        }

        public override void Write(Utf8JsonWriter writer, bool longValue, JsonSerializerOptions options)
        {
            writer.WriteStringValue(longValue.ToString());
        }
    }

}