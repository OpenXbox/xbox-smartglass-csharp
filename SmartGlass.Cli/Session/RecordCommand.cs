using System;
using System.Threading;
using System.Threading.Tasks;
using NClap.Metadata;

namespace SmartGlass.Cli.Session
{
    internal class RecordCommand : Command
    {
        [PositionalArgument(ArgumentFlags.Optional, Position = 0)]
        public int LastSeconds { get; set; } = 60;

        public override async Task<CommandResult> ExecuteAsync(CancellationToken cancel)
        {
            try
            {
                await ConnectCommand.Client.GameDvrRecord(LastSeconds);
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