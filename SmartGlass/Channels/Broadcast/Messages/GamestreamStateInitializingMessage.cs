using System;
using SmartGlass.Messaging.Session;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SmartGlass.Channels.Broadcast.Messages
{
    /// <summary>
    /// Gamestream state initializing message.
    /// Sent from console to client when console has finished setting up
    /// Nano and is ready for a client connection.
    /// </summary>
    [GamestreamStateMessageType(GamestreamStateMessageType.Initializing)]
    class GamestreamStateInitializingMessage : GamestreamStateBaseMessage
    {
        /// <summary>
        /// TCP port running the "Control protocol"
        /// </summary>
        /// <value>The TCP port.</value>
        public int TcpPort { get; set; }
        /// <summary>
        /// UDP port running the "Streaming protocol"
        /// </summary>
        /// <value>The UDP port.</value>
        public int UdpPort { get; set; }
    }
}
