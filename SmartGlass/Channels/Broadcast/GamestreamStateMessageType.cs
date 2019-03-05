using System;

namespace SmartGlass.Channels.Broadcast
{
    /// <summary>
    /// Gamestream state message type.
    /// </summary>
    enum GamestreamStateMessageType
    {
        Invalid,
        Initializing,
        Started,
        Stopped,
        Paused
    }
}
