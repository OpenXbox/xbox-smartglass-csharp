using System;
using System.Threading;
using System.Threading.Tasks;
using NClap.Metadata;

namespace DarkId.SmartGlass.Cli.Session
{
    internal class LaunchTitleCommand : Command
    {
        [PositionalArgument(ArgumentFlags.AtLeastOnce, Position = 0)]
        public uint TitleId { get; set; }

        [NamedArgument(ArgumentFlags.Optional)]
        public string Params { get; set; }

        public override async Task<CommandResult> ExecuteAsync(CancellationToken cancel)
        {
            try
            {
                await ConnectCommand.Client.LaunchTitleAsync(TitleId, Params);
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