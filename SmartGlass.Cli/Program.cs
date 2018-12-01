using NClap;
using SmartGlass.Common.Providers;

namespace SmartGlass.Cli
{
    internal class Program
    {
        static JsonConfigurationProvider Configuration;
        static int Main(string[] args)
        {
            var programArgs = new ProgramArguments();
            Configuration = new JsonConfigurationProvider(args);

            // extending default args
            args = Configuration.GetExtendedArgs();

            if (CommandLineParser.TryParse(args, out programArgs))
                return (int)programArgs.Command.Execute();

            return -1; // Error
        }
    }
}
