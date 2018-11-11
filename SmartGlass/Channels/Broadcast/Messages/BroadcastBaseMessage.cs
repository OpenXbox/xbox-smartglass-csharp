using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SmartGlass.Channels.Broadcast.Messages
{
    class BroadcastBaseMessage
    {
        public BroadcastMessageType Type { get; set; }
    }
}
