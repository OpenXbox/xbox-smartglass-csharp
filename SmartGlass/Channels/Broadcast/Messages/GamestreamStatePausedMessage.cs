using System;
using SmartGlass.Messaging.Session;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SmartGlass.Channels.Broadcast.Messages
{
    [GamestreamStateMessageType(GamestreamStateMessageType.Paused)]
    class GamestreamStatePausedMessage : GamestreamStateBaseMessage
    {
    }
}