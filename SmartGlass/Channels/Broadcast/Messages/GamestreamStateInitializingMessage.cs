using System;
using DarkId.SmartGlass.Messaging.Session;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DarkId.SmartGlass.Channels.Broadcast.Messages
{
    [GamestreamStateMessageType(GamestreamStateMessageType.Initializing)]
    class GamestreamStateInitializingMessage : GamestreamStateBaseMessage
    {
        public int TcpPort { get; set; }
        public int UdpPort { get; set; }
    }
}