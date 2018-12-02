using System;
using SmartGlass.Common;

namespace SmartGlass.Common
{
    // TODO: Error events.
    public class GamestreamSession
    {
        public Guid SessionId { get; private set; }
        public GamestreamConfiguration Config { get; private set; }
        public int TcpPort { get; private set; }
        public int UdpPort { get; private set; }

        public GamestreamSession(int tcpPort, int udpPort, GamestreamConfiguration config, Guid sessionId)
        {
            Config = config;
            SessionId = sessionId;
            TcpPort = tcpPort;
            UdpPort = udpPort;
        }
    }
}