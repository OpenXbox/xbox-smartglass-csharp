using System;
using System.Threading;
using System.Threading.Tasks;
using NClap.Metadata;
using System.Linq;
using DarkId.SmartGlass.Nano;

namespace DarkId.SmartGlass.Cli.Session
{
    class StartBroadcast : Command
    {
        public override async Task<CommandResult> ExecuteAsync(CancellationToken cancel)
        {
            try
            {
                var broadcastChannel = await ConnectCommand.Client.GetBroadcastChannelAsync();

                // TODO: Wait for BroadcastMessages here...

                var result = await broadcastChannel.StartGamestreamAsync();
                Console.WriteLine($"Connecting to TCP: {result.TcpPort}, UDP: {result.UdpPort}");
                var c = new NanoClient("10.0.0.241", result.TcpPort, result.UdpPort, new Guid());
                c.Initialize();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return CommandResult.RuntimeFailure;
            }

            return CommandResult.Success;
        }
    }
}