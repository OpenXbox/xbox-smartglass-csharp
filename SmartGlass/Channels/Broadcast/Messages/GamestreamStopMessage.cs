using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SmartGlass.Channels.Broadcast.Messages
{
    /// <summary>
    /// Gamestream stop message.
    /// Unsure which participant sends this.
    /// Normally disconnecting the Tcp/Udp sockets is sufficient to stop
    /// gamestreaming.
    /// </summary>
    class GamestreamStopMessage : BroadcastBaseMessage
    {
        public GamestreamStopMessage()
        {
            Type = BroadcastMessageType.StopGamestream;
        }
    }
}
