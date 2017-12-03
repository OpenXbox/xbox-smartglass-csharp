

using Microsoft.Extensions.Logging;

namespace DarkId.SmartGlass.Common
{
    public class Logging
    {
        public static ILoggerFactory Factory { get; private set; } =
            new LoggerFactory().AddDebug(LogLevel.Trace);
    }
}