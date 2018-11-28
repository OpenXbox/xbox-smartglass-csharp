using System;
using SmartGlass.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace SmartGlass.Channels.Broadcast
{
    class GamestreamConfigurationJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableTo(typeof(GamestreamConfiguration));
        }

        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader,
            Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer,
            object value, JsonSerializer serializer)
        {
            JsonSerializer camelCaseSerializer = new JsonSerializer()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            JObject obj = JObject.FromObject(value, camelCaseSerializer);

            foreach (var o in obj)
            {
                obj[o.Key] = o.Value.ToString().ToLower();
            }

            obj.WriteTo(writer);
        }
    }
}