namespace DarkId.SmartGlass.Messaging.Connection
{
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