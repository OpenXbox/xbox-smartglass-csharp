using System;

namespace SmartGlass
{
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