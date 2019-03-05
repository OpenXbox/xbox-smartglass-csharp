using System;
using SmartGlass.Common;

namespace SmartGlass.Common
{
    /// <summary>
    /// Gamestream session.
    /// Returned by BroadcastChannel when gamestreaming is initialized
    /// successfully.
    /// TODO: Error events.
    /// </summary>
    public class GamestreamSession
    {
        public Guid SessionId { get; private set; }
        public GamestreamConfiguration Config { get; private set; }
        public int TcpPort { get; private set; }
        public int UdpPort { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SmartGlass.Common.GamestreamSession"/> class.
        /// </summary>
        /// <param name="tcpPort">TCP port.</param>
        /// <param name="udpPort">UDP port.</param>
        /// <param name="config">Used gamestreaming configuration.</param>
        /// <param name="sessionId">Identifier of gamestream session.</param>
        public GamestreamSession(int tcpPort, int udpPort, GamestreamConfiguration config, Guid sessionId)
        {
            Config = config;
            SessionId = sessionId;
            TcpPort = tcpPort;
            UdpPort = udpPort;
        }
    }
}
