using System;
namespace SmartGlass.Nano
{
    public enum ControlOpCode : ushort
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
