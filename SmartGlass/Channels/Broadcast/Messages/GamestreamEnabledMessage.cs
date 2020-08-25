
namespace SmartGlass.Channels.Broadcast.Messages
{
    /// <summary>
    /// Informs the client about the availability of gamestream
    /// functionality and used protocol version.
    /// Sent from console to client when Broadcast channel is opened.
    /// </summary>
    [BroadcastMessageType(BroadcastMessageType.GamestreamEnabled)]
    class GamestreamEnabledMessage : BroadcastBaseMessage
    {
        /// <summary>
        /// Indicating whether gamestreaming is enabled on the console.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled { get; set; }
        /// <summary>
        /// Indicating whether gamestreaming can be enabled on the console.
        /// </summary>
        /// <value><c>true</c> if enabling is possible; otherwise, <c>false</c>.</value>
        public bool CanBeEnabled { get; set; }
        /// <summary>
        /// Major version of running Nano protocol.
        /// </summary>
        /// <value>The major protocol version.</value>
        public int MajorProtocolVersion { get; set; }
        /// <summary>
        /// Minor version of running Nano protocol.
        /// </summary>
        /// <value>The minor protocol version.</value>
        public int MinorProtocolVersion { get; set; }
    }
}
