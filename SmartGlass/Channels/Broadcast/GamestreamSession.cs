using System;
using System.Net.Sockets;
using DarkId.SmartGlass.Channels;

namespace DarkId.SmartGlass.Channels.Broadcast
{
    // TODO: Error events.
    public class GamestreamSession
    {
        public int TcpPort { get; private set; }
        public int UdpPort { get; private set; }

        internal GamestreamSession(int tcpPort, int udpPort)
        {
            TcpPort = tcpPort;
            UdpPort = udpPort;
        }
    }
}