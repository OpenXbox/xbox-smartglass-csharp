using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NClap.Metadata;

namespace DarkId.SmartGlass.Cli
{
    internal class PowerOnCommand : Command
    {
        [PositionalArgument(ArgumentFlags.Required, Position = 0)]
        public string Hostname { get; set; }

        [PositionalArgument(ArgumentFlags.Required, Position = 1)]
        public string LiveId { get; set; }

        public override async Task<CommandResult> ExecuteAsync(CancellationToken cancel)
        {
            Console.WriteLine($"Powering on {Hostname}...");

            Device device = null;
            try
            {
                device = await Device.PowerOnAsync(Hostname, LiveId);
                Console.WriteLine($"{device.Name} ({device.HardwareId}) {device.Address}");
            }
            catch (TimeoutException)
            {
                Console.WriteLine($"Failed to power on.  No response from {Hostname}.");
                return CommandResult.RuntimeFailure;
            }
            if (device == null)
            {
                return CommandResult.RuntimeFailure;
            }

            return CommandResult.Success;
        }
    }
}
