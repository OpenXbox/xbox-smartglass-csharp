

using Microsoft.Extensions.Logging;

namespace SmartGlass.Common
{
    /// <summary>
    /// Logging factory.
    /// </summary>
    public class Logging
    {
        public static ILoggerFactory Factory { get; set; } =
            LoggerFactory.Create(builder => builder.AddDebug().SetMinimumLevel(LogLevel.Trace));//note even if overwritten this will be called and thrown away
    }
}
