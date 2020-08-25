
namespace SmartGlass.Channels.Broadcast
{
    /// <summary>
    /// Broadcast message type.
    /// </summary>
    public enum BroadcastMessageType
    {
        StartGamestream = 1,
        StopGamestream,
        GamestreamState,
        GamestreamEnabled,
        GamestreamError,
        Telemetry,
        PreviewStatus
    }
}
