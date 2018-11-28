using System;
using SmartGlass.Channels.Broadcast.Messages;
using SmartGlass.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartGlass.Json;

namespace SmartGlass.Channels.Broadcast
{
    class BroadcastMessageJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableTo(typeof(BroadcastBaseMessage));
        }

        public override bool CanWrite => false;

        public override object ReadJson(JsonReader reader,
            Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);

            var messageType = (BroadcastMessageType)obj["type"].Value<int>();
            objectType = BroadcastMessageTypeAttribute.GetTypeForMessageType(messageType)
                ?? typeof(BroadcastBaseMessage);

            if (messageType == BroadcastMessageType.GamestreamState)
            {
                var stateMessageType = (GamestreamStateMessageType)obj["state"].Value<int>();
                objectType = GamestreamStateMessageTypeAttribute.GetTypeForMessageType(stateMessageType)
                    ?? typeof(GamestreamStateBaseMessage);
            }

            var guidSerializer = new JsonSerializer();
            guidSerializer.Converters.Add(new GuidConverter());

            return obj.ToObject(objectType, guidSerializer);
        }

        public override void WriteJson(JsonWriter writer,
            object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}