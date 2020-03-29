using Microsoft.Extensions.Logging;
using NClap;
using SmartGlass.Common;

namespace SmartGlass.Cli
{
    internal class Program
    {
        static internal ILogger logger = Logging.Factory.CreateLogger<Program>();
        static int Main(string[] args)
        {
            logger.LogTrace("Starting SmartGlass.Cli");

            var programArgs = new ProgramArguments();
            logger.LogTrace("Parsing CommandLine args");
            if (!CommandLineParser.TryParse(args, out programArgs))
            {
                logger.LogError("CommandLineParser.TryParse failed");
                return -1;
            }

            logger.LogTrace("Executing NClap program");
            return (int)programArgs.Command.Execute();
        }
    }
}
