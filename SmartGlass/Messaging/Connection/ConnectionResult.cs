namespace SmartGlass.Messaging.Connection
{
    /// <summary>
    /// Connection result returned in ConnectResponse.
    /// </summary>
    enum ConnectionResult : ushort
    {
        Success,
        Pending,
        FailureUnknown,
        FailureAnonymousConnectionsDisabled,
        FailureDeviceLimitExceeded,
        FailureSmartGlassDisabled,
        FailureUserAuthFailed,
        FailureUserSignInFailed,
        FailureUserSignInTimeOut,
        FailureUserSignInRequired
    }
}
