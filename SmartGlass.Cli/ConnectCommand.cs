using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SmartGlass.Cli.Session;
using XboxWebApi.Authentication;
using NClap.Metadata;
using NClap.Repl;

namespace SmartGlass.Cli
{
    internal class ConnectCommand : Command
    {
        // TODO: https://github.com/reubeno/NClap/issues/30
        public static SmartGlassClient Client { get; private set; }
        public static AuthenticationService AuthService { get; private set; }

        [PositionalArgument(ArgumentFlags.Required, Position = 0)]
        public string Hostname { get; set; }

        [PositionalArgument(ArgumentFlags.Optional, Position = 1)]
        public string TokenFilePath { get; set; }

        public override async Task<CommandResult> ExecuteAsync(CancellationToken cancel)
        {
            if (TokenFilePath != null)
            {
                AuthService = await AuthenticationService.LoadFromJsonFileAsync(TokenFilePath);
                await AuthService.AuthenticateAsync();
                await AuthService.DumpToJsonFileAsync(TokenFilePath);
            }

            Console.WriteLine($"Connecting to {Hostname}...");

            try
            {
                using (Client = await SmartGlassClient.ConnectAsync(Hostname,
                            AuthService == null ? null : AuthService.XToken.UserInformation.Userhash,
                            AuthService == null ? null : AuthService.XToken.Jwt))
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