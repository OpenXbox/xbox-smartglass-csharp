using System;
using System.Threading;
using System.Threading.Tasks;
using NClap.Metadata;

namespace SmartGlass.Cli.Session
{
    using System.Linq;

    internal class MediaCommand : Command
    {
        [PositionalArgument(ArgumentFlags.AtMostOnce, Position = 0)]
        public string Command { get; set; }

        public override async Task<CommandResult> ExecuteAsync(CancellationToken cancel) {
            var activeTitle =
                ConnectCommand.Client
                    .CurrentConsoleStatus
                    .ActiveTitles
                    .FirstOrDefault()
                  ?.TitleId ?? 0U;

            if (activeTitle == 0) {
                Console.WriteLine("Don't know the active title; skipping...");
                return CommandResult.RuntimeFailure;
            }

            var state = new MediaCommandState();
            state.TitleId = activeTitle;

            var parsed = Enum.TryParse<MediaControlCommands>(Command, true, out state.Command);
            if (!parsed)
            {
                return CommandResult.UsageError;
            }

            try
            {
                var mediaChannel = ConnectCommand.Client.MediaChannel;

                await mediaChannel.SendMediaCommandStateAsync(state);
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
