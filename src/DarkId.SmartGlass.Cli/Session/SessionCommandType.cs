using NClap.Metadata;

namespace DarkId.SmartGlass.Cli.Session
{
    enum SessionCommandType
    {
        [Command(
            typeof(ExitCommand),
            Description = "Disconnect from the console.")]
        Exit,

        [Command(typeof(PressCommand))]
        Press,

        [Command(typeof(LaunchTitleCommand))]
        LaunchTitle,

        [Command(typeof(TitleChannelCommand))]
        TitleChannel,

        // [Command(typeof(RecordCommand))]
        // Record,

        [HelpCommand]
        Help
    }
}