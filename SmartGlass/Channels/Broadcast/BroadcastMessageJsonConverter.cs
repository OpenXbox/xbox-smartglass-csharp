using System;
using SmartGlass.Channels.Broadcast.Messages;
using SmartGlass.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

            if (messageType == BroadcastMessageType.GamestreamState)
            {
                var stateMessageType = (GamestreamStateMessageType)obj["state"].Value<int>();
                return obj.ToObject(GamestreamStateMessageTypeAttribute.GetTypeForMessageType(stateMessageType)
                    ?? typeof(GamestreamStateBaseMessage));
            }

            return obj.ToObject(BroadcastMessageTypeAttribute.GetTypeForMessageType(messageType)
                ?? typeof(BroadcastBaseMessage));
        }

        public override void WriteJson(JsonWriter writer,
            object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}