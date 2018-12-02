using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SmartGlass.Json;
using SmartGlass.Channels.Broadcast;

namespace SmartGlass.Channels
{
    static class ChannelJsonSerializerSettings
    {
        public static JsonSerializerSettings GetBroadcastSettings()
        {
            var serializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            serializerSettings.Converters.Add(new GuidConverter());
            serializerSettings.Converters.Add(new BroadcastMessageJsonConverter());
            serializerSettings.Converters.Add(new GamestreamConfigurationJsonConverter());

            return serializerSettings;
        }
    }
}
