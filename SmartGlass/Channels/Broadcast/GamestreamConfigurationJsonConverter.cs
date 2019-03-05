using System;
using SmartGlass.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace SmartGlass.Channels.Broadcast
{
    /// <summary>
    /// GamestreamConfiguration json converter.
    /// </summary>
    class GamestreamConfigurationJsonConverter : JsonConverter
    {
        /// <summary>
        /// Checks wether the object type can be converted to json
        /// </summary>
        /// <returns><c>true</c>, if convert is possible, <c>false</c> otherwise.</returns>
        /// <param name="objectType">Object type.</param>
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableTo(typeof(GamestreamConfiguration));
        }

        /// <summary>
        /// Gets a value indicating whether this
        /// <see cref="T:SmartGlass.Channels.Broadcast.GamestreamConfigurationJsonConverter"/> can write.
        /// </summary>
        /// <value><c>true</c> if can write; otherwise, <c>false</c>.</value>
        public override bool CanWrite => true;

        /// <summary>
        /// Invoked by Newtonsoft Json library.
        /// Reads the json and deserializes into Gamestream/Broadcast message.
        /// </summary>
        /// <returns>Appropriate message object.</returns>
        /// <param name="reader">Json reader.</param>
        /// <param name="objectType">Object type.</param>
        /// <param name="existingValue">Existing value.</param>
        /// <param name="serializer">Serializer.</param>
        public override object ReadJson(JsonReader reader,
            Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new NotImplementedException();
        }

        /// <summary>
        /// Invoked by Newtonsoft Json library.
        /// Reads the object and writes out a GamestreamConfiguration json string.
        /// </summary>
        /// <returns>JSON string.</returns>
        /// <param name="writer">Json writer.</param>
        /// <param name="value">Object to serialize.</param>
        /// <param name="serializer">Serializer.</param>
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
