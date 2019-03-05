using System;
using SmartGlass.Messaging.Session;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SmartGlass.Channels.Broadcast.Messages
{
    /// <summary>
    /// Gamestream state started message.
    /// Sent from console to client when gamestreaming has started.
    /// </summary>
    [GamestreamStateMessageType(GamestreamStateMessageType.Started)]
    class GamestreamStateStartedMessage : GamestreamStateBaseMessage
    {
        /// <summary>
        /// Indicating whether console is connected to the nework via a wireless connection.
        /// </summary>
        /// <value><c>true</c> if wireless connection is used; otherwise, <c>false</c>.</value>
        public bool IsWirelessConnection { get; set; }
        /// <summary>
        /// Wireless channel the console is connected with, in case of wireless connection.
        /// </summary>
        /// <value>Used wireless channel.</value>
        public int WirelessChannel { get; set; }
        /// <summary>
        /// Network transmit link speed.
        /// </summary>
        /// <value>The transmit link speed.</value>
        public int TransmitLinkSpeed { get; set; }
    }
}
