using NClap;

namespace DarkId.SmartGlass.Cli
{
    internal class Program
    {
        // TODO: Redesign command/REPL structure in order to execute session based commands outside of REPL.

        static int Main(string[] args)
        {
            var programArgs = new ProgramArguments();
            if (!CommandLineParser.ParseWithUsage(args, programArgs))
            {
                return -1;
            }

            return (int)programArgs.Command.Execute();
        }
    }
}
