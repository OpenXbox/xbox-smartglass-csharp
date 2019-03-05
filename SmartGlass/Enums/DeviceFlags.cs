using System;

namespace SmartGlass
{
    /// <summary>
    /// Device flags contained in PresenceResponse.
    /// </summary>
    [Flags]
    public enum DeviceFlags
    {
        None,
        AllowConsoleUsers = 1,
        AllowAuthenticatedUsers = 2,
        AllowAnonymousUsers = 4,
        CertificateRequestPending = 8
    }
}
