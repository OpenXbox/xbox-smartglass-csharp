using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DarkId.SmartGlass.Channels.Broadcast.Messages
{
    class GamestreamStateBaseMessage : BroadcastBaseMessage
    {
        public GamestreamStateMessageType State { get; set; }
        public Guid SessionId { get; set; }
    }
}
