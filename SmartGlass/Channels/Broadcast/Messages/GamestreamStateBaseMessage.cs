using System;

namespace SmartGlass.Channels.Broadcast.Messages
{
    /// <summary>
    /// Gamestream state base message.
    /// </summary>
    class GamestreamStateBaseMessage : BroadcastBaseMessage
    {
        /// <summary>
        /// Gamestream state.
        /// State messages are sent from console to client.
        /// </summary>
        /// <value>The state.</value>
        public GamestreamStateMessageType State { get; set; }
        /// <summary>
        /// Current session identifier.
        /// </summary>
        /// <value>The session identifier.</value>
        public Guid SessionId { get; set; }
    }
}
