namespace DarkId.SmartGlass.Messaging.Session
{
    enum SessionMessageType : ushort
    {
        Ack = 0x01,
        Group = 0x02, // Unused
        LocalJoin = 0x03,
        StopActivity = 0x05, // Unused
        AuxiliaryStream = 0x19,
        ActiveSurfaceChange = 0x1A,
        Navigate = 0x1B, // Unused
        Json = 0x1C,
        Tunnel = 0x1D, // Unused
        ConsoleStatus = 0x1E,
        TitleTextConfiguration = 0x1F,
        TitleTextInput = 0x20,
        TitleTextSelection = 0x21,
        MirroringRequest = 0x22,
        TitleLaunch = 0x23,
        StartChannelRequest = 0x26,
        StartChannelResponse = 0x27,
        StopChannel = 0x28,
        System = 0x29, // Unused
        Disconnect = 0x2A,
        TitleTouch = 0x2E,
        Accelerometer = 0x2F,
        Gyrometer = 0x30,
        Inclinometer = 0x31,
        Compass = 0x32,
        Orientation = 0x33,
        PairedIdentityStateChanged = 0x36,
        Unsnap = 0x37, // Unused
        GameDvrRecord = 0x38,
        PowerOff = 0x39,

        MediaControllerRemoved = 0xF00,
        MediaCommand = 0xF01,
        MediaCommandResult = 0xF02,
        MediaState = 0xF03,
        Gamepad = 0xF0A,
        SystemTextConfiguration = 0xF2B,
        SystemTextInput = 0xF2C,
        SystemTouch = 0xF2E,
        SystemTextAcknowledge = 0xF34,
        SystemTextDone = 0xF35
    }
}