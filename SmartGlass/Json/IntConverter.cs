using System;
using System.Buffers;
using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartGlass.Json
{
    public class IntConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
                if (Utf8Parser.TryParse(span, out int number, out int bytesConsumed) && span.Length == bytesConsumed)
                    return number;

                if (Int32.TryParse(reader.GetString(), out number))
                    return number;
            }

            return reader.GetInt32();
        }

        public override void Write(Utf8JsonWriter writer, int longValue, JsonSerializerOptions options)
        {
            writer.WriteStringValue(longValue.ToString());
        }
    }
}