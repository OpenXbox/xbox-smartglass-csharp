using System;
namespace DarkId.SmartGlass.Nano
{
    public enum ControlPayloadType
    {
        SessionInit = 1,
        SessionCreate,
        SessionCreateResponse,
        SessionDestroy,
        VideoStatistics,
        RealtimeTelemetry,
        ChangeVideoQuality,
        InitiateNetworkTest,
        NetworkInformation,
        NetworkTestResponse,
        ControllerEvent
    }
}
