using System;
using System.Threading;
using System.Threading.Tasks;
using NClap.Metadata;

namespace SmartGlass.Cli.Session
{
    internal class PressCommand : Command
    {
        [PositionalArgument(ArgumentFlags.AtLeastOnce, Position = 0)]
        public string[] Buttons { get; set; }

        public override async Task<CommandResult> ExecuteAsync(CancellationToken cancel)
        {
            var state = new GamepadState();

            var parsed = Enum.TryParse<GamepadButtons>(string.Join(", ", Buttons), true, out state.Buttons);
            if (!parsed)
            {
                return CommandResult.UsageError;
            }

            try
            {
                var inputChannel = await ConnectCommand.Client.GetInputChannelAsync();

                await inputChannel.SendGamepadStateAsync(state);
                await Task.Delay(100);
                await inputChannel.SendGamepadStateAsync(new GamepadState()
                {
                    Buttons = GamepadButtons.Clear
                });
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