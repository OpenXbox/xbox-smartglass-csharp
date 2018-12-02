using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NClap.Metadata;
using NClap.Repl;

using XboxWebApi;
using XboxWebApi.Authentication;
using XboxWebApi.Authentication.Model;


namespace SmartGlass.Cli
{
    internal class AuthenticateCommand : Command
    {
        [PositionalArgument(ArgumentFlags.Required, Position = 0)]
        public string TokenFilePath { get; set; }

#pragma warning disable 1998
        public override async Task<CommandResult> ExecuteAsync(CancellationToken cancel)
        {
            string authUrl = AuthenticationService.GetWindowsLiveAuthenticationUrl();

            Console.WriteLine($"Go to following URL and authenticate: {authUrl}");
            Console.WriteLine("Paste the returned URL and press ENTER: ");

            string redirectUrl = Console.ReadLine();

            try
            {
                WindowsLiveResponse response = AuthenticationService
                    .ParseWindowsLiveResponse(redirectUrl);

                AuthenticationService authService = new AuthenticationService(response);

                authService.Authenticate();

                using (FileStream fs = File.Open(TokenFilePath, FileMode.Create))
                {
                    authService.DumpToFile(fs);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Authentication failed! {e.Message}");
                return CommandResult.RuntimeFailure;
            }

            Console.WriteLine($"Authentication succeeded, tokens saved to {TokenFilePath}");
            return CommandResult.Success;
        }
#pragma warning restore 1998
    }
}