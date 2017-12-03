using NClap.Metadata;

namespace DarkId.SmartGlass.Cli
{
    enum MainCommandType
    {
        [Command(
            typeof(ConnectCommand),
            Description = "Opens a connection to an Xbox One console.")]
        Connect,

        [Command(
            typeof(DiscoverCommand),
            Description = "Discover and list Xbox One consoles on the local network.")]
        Discover,

        [Command(
            typeof(PingCommand),
            Description = "Ping and output details of an Xbox One console.")]
        Ping
    }
}