using System;

namespace SmartGlass.Channels.Broadcast.Messages
{
    class GamestreamStateBaseMessage : BroadcastBaseMessage
    {
        public GamestreamStateMessageType State { get; set; }
        public Guid SessionId { get; set; }
    }
}
