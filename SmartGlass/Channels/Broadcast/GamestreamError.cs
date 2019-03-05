using System;

namespace SmartGlass.Channels.Broadcast
{
    /// <summary>
    /// Gamestream error.
    /// </summary>
    public enum GamestreamError
    {
        General = 1,
        FailedToInstantiate,
        FailedToInitialize,
        FailedToStart,
        FailedToStop,
        NoController,
        DifferentMSAactive,
        DrmVideo,
        HdcpVideo,
        KinectTitle,
        ProhibitedGame,
        PoorNetworkConnection,
        StreamingDisabled,
        CannotReachConsole,
        GenericError,
        VersionMismatch,
        NoProfile,
        BroadcastInProgress
    }
}
