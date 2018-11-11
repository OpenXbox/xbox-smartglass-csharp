using System;
using System.Threading;
using System.Threading.Tasks;
using DarkId.SmartGlass.Cli.Session;
using NClap.Metadata;
using NClap.Repl;

namespace DarkId.SmartGlass.Cli
{
    internal class ConnectCommand : Command
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