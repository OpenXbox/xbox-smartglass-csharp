using System;
using System.Threading;
using System.Threading.Tasks;
using SmartGlass.Nano;
using SmartGlass.Cli.Session;
using NClap.Metadata;
using NClap.Repl;

namespace SmartGlass.Cli
{
    internal class BroadcastCommand : Command
    {
        // TODO: https://github.com/reubeno/NClap/issues/30
        public static SmartGlassClient Client { get; private set; }

        [PositionalArgument(ArgumentFlags.Required, Position = 0)]
        public string Hostname { get; set; }

        public override async Task<CommandResult> ExecuteAsync(CancellationToken cancel)
        {
            Console.WriteLine($"Connecting to {Hostname}...");

            try
            {
                using (Client = await SmartGlassClient.ConnectAsync(Hostname))
                {
                    var broadcastChannel = Client.BroadcastChannel;
                    // TODO: Wait for BroadcastMessages here...

                    var result = await broadcastChannel.StartGamestreamAsync();
                    Console.WriteLine($"Connecting to Nano, TCP: {result.TcpPort}, UDP: {result.UdpPort}");
                    var nano = new NanoClient(Hostname, result.TcpPort, result.UdpPort, new Guid());
                    await nano.Initialize();

                    var loop = new Loop(typeof(SessionCommandType));
                    loop.Execute();

                    Console.WriteLine($"Disconnected");
                }

                return CommandResult.Success;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to connect: {e}");
            }
            finally
            {
                Client = null;
            }

            return CommandResult.RuntimeFailure;
        }
    }
}
