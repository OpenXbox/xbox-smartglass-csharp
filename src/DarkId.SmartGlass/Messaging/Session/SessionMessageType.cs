namespace DarkId.SmartGlass.Messaging.Session
{
    enum SessionMessageType : ushort
    {
        Ack = 1,
        LocalJoin = 3,
        AuxiliaryStream = 25,
        ActiveSurfaceChange = 26,
        Json = 28,
        ConsoleStatus = 30,
        TitleLaunch = 35,
        StartChannelRequest = 38,
        StartChannelResponse = 39,
        StopChannel = 40,
        Disconnect = 42,
        GameDvrRecord = 56,
        PowerOff = 57,
        Gamepad = 3850
    }
}