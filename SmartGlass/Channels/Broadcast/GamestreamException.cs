
namespace SmartGlass.Channels.Broadcast
{
    /// <summary>
    /// Gamestream exception.
    /// </summary>
    public class GamestreamException : SmartGlassException
    {
        public GamestreamException(string message, GamestreamError result)
            : base(message, (int)result)
        {
        }

        /// <summary>
        /// Gets the specific GamestreamError.
        /// </summary>
        /// <value>The error.</value>
        public GamestreamError Error => (GamestreamError)HResult;
    }
}
