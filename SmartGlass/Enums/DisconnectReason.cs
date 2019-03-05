namespace SmartGlass
{
    /// <summary>
    /// Disconnect reason.
    /// </summary>
    enum DisconnectReason
    {
        Unspecified,
        Error,
        PowerOff,
        Maintenance,
        AppClose,
        SignOut,
        Reboot,
        Disabled,
        LowPower
    }
}
