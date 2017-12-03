namespace DarkId.SmartGlass.Messaging.Session.Messages
{
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