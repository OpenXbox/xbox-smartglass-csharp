using System;
using SmartGlass.Messaging.Session;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SmartGlass.Channels.Broadcast.Messages
{
    /// <summary>
    /// Gamestream state stopped message.
    /// Sent from console to client when gamestreaming is stopped.
    /// </summary>
    [GamestreamStateMessageType(GamestreamStateMessageType.Stopped)]
    class GamestreamStateStoppedMessage : GamestreamStateBaseMessage
    {
    }
}
