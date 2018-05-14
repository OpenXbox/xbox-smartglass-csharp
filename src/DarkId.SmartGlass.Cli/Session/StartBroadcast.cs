using System;
using System.Threading;
using System.Threading.Tasks;
using NClap.Metadata;
using System.Linq;

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

                await broadcastChannel.StartGamestreamAsync();
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