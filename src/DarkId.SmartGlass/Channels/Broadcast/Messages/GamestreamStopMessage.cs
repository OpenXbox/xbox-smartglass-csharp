using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DarkId.SmartGlass.Channels.Broadcast.Messages
{
    class GamestreamStopMessage : BroadcastBaseMessage
    {
        public GamestreamStopMessage()
        {
            Type = BroadcastMessageType.StopGamestream;
        }
    }
}
