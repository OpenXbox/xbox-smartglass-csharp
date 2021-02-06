

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

namespace SmartGlass.Common
{
    /// <summary>
    /// Logging factory.
    /// </summary>
    public class Logging
    {
        public static ILoggerFactory Factory { get; private set; } =
            LoggerFactory.Create(builder => builder.AddDebug().SetMinimumLevel(LogLevel.Trace));
    }
}
