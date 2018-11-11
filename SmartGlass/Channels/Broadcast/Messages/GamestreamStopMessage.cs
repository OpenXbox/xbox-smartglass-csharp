using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SmartGlass.Channels.Broadcast.Messages
{
    class GamestreamStopMessage : BroadcastBaseMessage
    {
        public GamestreamStopMessage()
        {
            Type = BroadcastMessageType.StopGamestream;
        }
    }
}
