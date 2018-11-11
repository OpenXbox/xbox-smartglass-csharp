namespace DarkId.SmartGlass
{
    enum DeviceCapabilities : long
    {
        SupportsAll = -1,
        SupportsNone = 0,
        SupportsStreaming = 1,
        SupportsAudio = 2,
        SupportsAccelerometer = 4,
        SupportsCompass = 8,
        SupportsGyrometer = 16,
        SupportsInclinometer = 32,
        SupportsOrientation = 64
    }
}