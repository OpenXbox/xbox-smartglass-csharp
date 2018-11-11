using System;

namespace DarkId.SmartGlass.Channels.Broadcast.Messages
{
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
