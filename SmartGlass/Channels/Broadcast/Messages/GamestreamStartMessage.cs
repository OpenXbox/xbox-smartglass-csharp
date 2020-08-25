using SmartGlass.Common;

namespace SmartGlass.Channels.Broadcast.Messages
{
    /// <summary>
    /// Gamestream start message.
    /// Sent from client to console to initialize Gamestream.
    /// </summary>
    [BroadcastMessageType(BroadcastMessageType.StartGamestream)]
    class GamestreamStartMessage : BroadcastBaseMessage
    {
        /// <summary>
        /// Desired Gamestream configuration
        /// </summary>
        /// <value>The configuration.</value>
        public GamestreamConfiguration Configuration { get; set; }
        /// <summary>
        /// Indicating whether client wants to re-query console's preview status.
        /// </summary>
        /// <value>
        /// <c>true</c> if re-querying preview status is desired,
        /// otherwise, <c>false</c>.
        /// </value>
        public bool ReQueryPreviewStatus { get; set; }

        public GamestreamStartMessage()
        {
            Type = BroadcastMessageType.StartGamestream;
        }
    }
}
