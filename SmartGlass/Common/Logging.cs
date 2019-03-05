

using Microsoft.Extensions.Logging;

namespace SmartGlass.Common
{
    /// <summary>
    /// Logging factory.
    /// </summary>
    public class Logging
    {
        public static ILoggerFactory Factory { get; private set; } =
            new LoggerFactory().AddDebug(LogLevel.Trace);
    }
}
