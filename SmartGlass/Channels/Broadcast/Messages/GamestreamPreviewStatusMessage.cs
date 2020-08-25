
namespace SmartGlass.Channels.Broadcast.Messages
{
    /// <summary>
    /// Gamestream preview status message.
    /// Sent from console to client.
    /// </summary>
    [BroadcastMessageType(BroadcastMessageType.PreviewStatus)]
    class GamestreamPreviewStatusMessage : BroadcastBaseMessage
    {
        /// <summary>
        /// Iindicating whether console is running public preview (Nano or OS?).
        /// </summary>
        /// <value><c>true</c> if running public preview; otherwise, <c>false</c>.</value>
        public bool IsPublicPreview { get; set; }
        /// <summary>
        /// Iindicating whether console is running internal preview (Nano or OS?).
        /// </summary>
        /// <value><c>true</c> if running internal preview; otherwise, <c>false</c>.</value>
        public bool IsInternalPreview { get; set; }
    }
}
