using System;

namespace SmartGlass.Channels.Broadcast
{
    /// <summary>
    /// Broadcast message type.
    /// </summary>
    enum BroadcastMessageType
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
