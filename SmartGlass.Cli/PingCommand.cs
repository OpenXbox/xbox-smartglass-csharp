using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NClap.Metadata;

namespace DarkId.SmartGlass.Cli
{
    internal class PingCommand : Command
    {
        [PositionalArgument(ArgumentFlags.AtLeastOnce, Position = 0)]
        public string[] Hostnames { get; set; }

        public override async Task<CommandResult> ExecuteAsync(CancellationToken cancel)
        {
            await Task.WhenAll(Hostnames.Select(async hostname =>
            {
                try
                {
                    var device = await Device.PingAsync(hostname);
                    Console.WriteLine($"{device.Name} ({device.HardwareId}) {device.Address}");
                }
                catch (TimeoutException)
                {
                    Console.WriteLine($"No response from {hostname}");
                }
            }));

            return CommandResult.Success;
        }
    }
}