using System;

namespace DarkId.SmartGlass.Channels.Broadcast
{
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
