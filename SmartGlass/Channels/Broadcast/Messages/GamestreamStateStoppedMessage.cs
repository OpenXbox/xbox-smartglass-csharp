
namespace SmartGlass.Channels.Broadcast.Messages
{
    /// <summary>
    /// Gamestream state stopped message.
    /// Sent from console to client when gamestreaming is stopped.
    /// </summary>
    [GamestreamStateMessageType(GamestreamStateMessageType.Stopped)]
    class GamestreamStateStoppedMessage : GamestreamStateBaseMessage
    {
    }
}
