using System;
using SmartGlass.Messaging.Session;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SmartGlass.Channels.Broadcast.Messages
{
    [GamestreamStateMessageType(GamestreamStateMessageType.Started)]
    class GamestreamStateStartedMessage : GamestreamStateBaseMessage
    {
        public bool IsWirelessConnection { get; set; }
        public int WirelessChannel { get; set; }
        public int TransmitLinkSpeed { get; set; }
    }
}