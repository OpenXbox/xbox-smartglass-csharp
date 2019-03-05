using System;
using SmartGlass.Messaging.Session;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SmartGlass.Channels.Broadcast.Messages
{
    /// <summary>
    /// Gamestream state paused message.
    /// Sent from console to client when gamestreaming is paused.
    /// </summary>
    [GamestreamStateMessageType(GamestreamStateMessageType.Paused)]
    class GamestreamStatePausedMessage : GamestreamStateBaseMessage
    {
    }
}
