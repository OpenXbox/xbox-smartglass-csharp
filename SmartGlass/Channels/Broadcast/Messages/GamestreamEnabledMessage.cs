using System;
using SmartGlass.Messaging.Session;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SmartGlass.Channels.Broadcast.Messages
{
    [BroadcastMessageType(BroadcastMessageType.GamestreamEnabled)]
    class GamestreamEnabledMessage : BroadcastBaseMessage
    {
        public bool Enabled { get; set; }
        public bool CanBeEnabled { get; set; }
        public int MajorProtocolVersion { get; set; }
        public int MinorProtocolVersion { get; set; }
    }
}