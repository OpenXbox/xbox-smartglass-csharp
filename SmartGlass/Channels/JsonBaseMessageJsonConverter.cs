using System;
using SmartGlass.Channels.Broadcast.Messages;
using SmartGlass.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartGlass.Json;

namespace SmartGlass.Channels {
    /// <summary>
    /// Broadcast message json converter.
    /// </summary>
    class JsonBaseMessageJsonConverter : JsonConverter
    {
        /// <summary>
        /// Checks wether the object type can be converted to json
        /// </summary>
        /// <returns><c>true</c>, if convert is possible, <c>false</c> otherwise.</returns>
        /// <param name="objectType">Object type.</param>
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableTo(typeof (BroadcastBaseMessage));
        }

        /// <summary>
        /// Gets a value indicating whether this
        /// <see cref="T:SmartGlass.Channels.Broadcast.JsonBaseMessageJsonConverter"/> can write.
        /// </summary>
        /// <value><c>true</c> if can write; otherwise, <c>false</c>.</value>
        public override bool CanWrite => false;

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
            var obj = JObject.Load(reader);

            objectType = typeof(JsonBaseMessage);


            var guidSerializer = new JsonSerializer();
            guidSerializer.Converters.Add(new GuidConverter());

            return obj.ToObject(objectType, guidSerializer);
        }

        /// <summary>
        /// Writes the json.
        /// </summary>
        /// <param name="writer">Writer.</param>
        /// <param name="value">Value.</param>
        /// <param name="serializer">Serializer.</param>
        public override void WriteJson(JsonWriter writer,
            object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
