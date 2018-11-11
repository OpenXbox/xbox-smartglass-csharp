using System;
using System.Threading;
using System.Threading.Tasks;
using NClap.Metadata;

namespace SmartGlass.Cli.Session
{
    internal class PowerOffCommand : Command
    {
        public override async Task<CommandResult> ExecuteAsync(CancellationToken cancel)
        {
            try
            {
                await ConnectCommand.Client.PowerOffAsync();
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