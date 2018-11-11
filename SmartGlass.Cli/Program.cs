using NClap;

namespace SmartGlass.Cli
{
    internal class Program
    {
        static int Main(string[] args)
        {
            var programArgs = new ProgramArguments();
            if (!CommandLineParser.TryParse(args, out programArgs))
            {
                return -1;
            }

            return (int)programArgs.Command.Execute();
        }
    }
}
