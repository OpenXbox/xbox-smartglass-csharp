

using Microsoft.Extensions.Logging;

namespace SmartGlass.Common
{
    /// <summary>
    /// Logging factory.
    /// </summary>
    public class Logging
    {
        public static ILoggerFactory Factory { get; set; } =
            LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter(logLevel => logLevel >= LogLevel.Trace)
                    .AddDebug();
            });
    }
}
