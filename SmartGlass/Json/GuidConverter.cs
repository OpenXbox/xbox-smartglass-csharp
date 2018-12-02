using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartGlass.Json
{
    // We use this to gracefully handle cases where Guids are sent as empty strings.
    //
    // Thanks, carlosfigueira!
    // https://stackoverflow.com/a/10114142/146765
    class GuidConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Guid) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // TODO: check why Guid.empty is casueing bad issues afterwards..
            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    return Guid.Empty;

                case JsonToken.String:
                    var str = reader.Value as string;
                    if (string.IsNullOrWhiteSpace(str))
                    {
                        return Guid.Empty;
                    }
                    else
                    {
                        return new Guid(str);
                    }

                default:
                    throw new ArgumentException("Invalid token type");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (Guid.Empty.Equals(value))
            {
                writer.WriteValue("");
            }
            else
            {
                writer.WriteValue((Guid)value);
            }
        }
    }
}
