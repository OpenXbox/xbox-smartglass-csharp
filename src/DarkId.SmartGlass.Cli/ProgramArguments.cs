using NClap.Metadata;

namespace DarkId.SmartGlass.Cli
{
    [ArgumentSet(
        AnswerFileArgumentPrefix = null,
        Style = ArgumentSetStyle.GetOpt,
        NameGenerationFlags = ArgumentNameGenerationFlags.GenerateHyphenatedLowerCaseLongNames |
                              ArgumentNameGenerationFlags.PreferLowerCaseForShortNames,
        Description = "Discover and control Xbox One consoles.")]
    internal class ProgramArguments : HelpArgumentsBase
    {
        [PositionalArgument(ArgumentFlags.Required, Position = 0)]
        public CommandGroup<MainCommandType> Command { get; set; }
    }
}