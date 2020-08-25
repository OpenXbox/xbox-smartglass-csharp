using SmartGlass.Channels.Broadcast;
using SmartGlass.Json;
using System.Text.Json;

namespace SmartGlass.Channels
{
    public class ChannelJsonSerializerOptions
    {
        public static JsonSerializerOptions GetBroadcastOptions(bool intended = false)
        {
            var serializerOptions = new JsonSerializerOptions() { WriteIndented = intended, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            serializerOptions.Converters.Add(new BroadcastMessageJsonConverter());
            serializerOptions.Converters.Add(new IntConverter());
            serializerOptions.Converters.Add(new FloatConverter());
            serializerOptions.Converters.Add(new BooleanConverter());
            serializerOptions.Converters.Add(new GuidConverter());
            serializerOptions.Converters.Add(new JsonBaseMessageJsonConverter());
            return serializerOptions;
        }

    }
}
