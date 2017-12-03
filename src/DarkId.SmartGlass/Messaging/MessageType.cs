namespace DarkId.SmartGlass.Messaging
{
    enum MessageType : ushort
    {
        SessionMessage = 53261,

        ConnectRequest = 52224,
        ConnectResponse = 52225,

        PresenceRequest = 56576,
        PresenceResponse = 56577,

        PowerOn = 56578
    }
}