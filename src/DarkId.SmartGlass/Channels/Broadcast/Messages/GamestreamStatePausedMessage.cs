using System;
using DarkId.SmartGlass.Messaging.Session;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DarkId.SmartGlass.Channels.Broadcast.Messages
{
    [GamestreamStateMessageType(GamestreamStateMessageType.Paused)]
    class GamestreamStatePausedMessage : GamestreamStateBaseMessage
    {
    }
}