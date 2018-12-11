using System;

namespace SmartGlass.Common
{
    public static class DateTimeExtensions
    {
        public static DateTime EpochDt => new DateTime(1970, 1, 1);

        /// <summary>
        /// Convert milliseconds since epoch to DateTime in UTC format
        /// </summary>
        /// <param name="dt">DateTime</param>
        /// <param name="milliseconds">Milliseconds since epoch</param>
        /// <returns>DateTime in UTC format</returns>
        public static DateTime FromEpochMillisecondsUtc(this DateTime dt, ulong milliseconds)
        {
            return EpochDt.AddMilliseconds(milliseconds).ToUniversalTime();
        }

        /// <summary>
        /// Convert seconds since epoch to DateTime in UTC format
        /// </summary>
        /// <param name="dt">DateTime</param>
        /// <param name="seconds">Seconds since epoch</param>
        /// <returns>DateTime in UTC format</returns>
        public static DateTime FromEpochSecondsUtc(this DateTime dt, ulong seconds)
        {
            return EpochDt.AddSeconds(seconds).ToUniversalTime();
        }
    }
}