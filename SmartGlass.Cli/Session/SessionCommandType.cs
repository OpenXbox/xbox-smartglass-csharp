using NClap.Metadata;

namespace SmartGlass.Cli.Session
{
    internal enum SessionCommandType
    {
        [Command(
            typeof(ExitCommand),
            Description = "Disconnect from the console.")]
        Exit,

        [Command(typeof(MediaCommand))]
        Media,

        [Command(typeof(PressCommand))]
        Press,

        [Command(typeof(TitleChannelCommand))]
        TitleChannel,

        [Command(typeof(RecordCommand))]
        Record,

        [Command(typeof(PowerOffCommand))]
        PowerOff,

        [HelpCommand]
        Help
    }
}