namespace SmartGlass.Messaging
{
    /// <summary>
    /// Message type.
    /// </summary>
    enum MessageType : ushort
    {
        SessionMessage = 0xD00D,

        ConnectRequest = 0xCC00,
        ConnectResponse = 0xCC01,

        PresenceRequest = 0xDD00,
        PresenceResponse = 0xDD01,

        PowerOn = 0xDD02
    }
}
