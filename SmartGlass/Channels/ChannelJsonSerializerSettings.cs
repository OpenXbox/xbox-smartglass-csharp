using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SmartGlass.Json;
using SmartGlass.Channels.Broadcast;

namespace SmartGlass.Channels
{
    /// <summary>
    /// Json serializer settings for ServiceChannels
    /// </summary>
    static class ChannelJsonSerializerSettings
    {
        /// <summary>
        /// Gets the JSON serializer settings for Broadcast channel.
        /// </summary>
        /// <returns>JSON serializer settings</returns>
        public static JsonSerializerSettings GetBroadcastSettings()
        {
            var serializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            serializerSettings.Converters.Add(new GuidConverter());
            serializerSettings.Converters.Add(new BroadcastMessageJsonConverter());
            serializerSettings.Converters.Add(new GamestreamConfigurationJsonConverter());
            serializerSettings.Converters.Add(new JsonBaseMessageJsonConverter());

            return serializerSettings;
        }
    }
}
