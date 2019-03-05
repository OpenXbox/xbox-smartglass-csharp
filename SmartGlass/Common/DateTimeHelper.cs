using System;

namespace SmartGlass.Common
{
    /// <summary>
    /// DateTime helper functions.
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Gets the UNIX epoch of 1970-01-01 (UTC) as DateTime
        /// </summary>
        /// <value>Epoch DateTime.</value>
        public static DateTime EpochDt => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Convert milliseconds since epoch to DateTime.
        /// DateTimeKind is UniversalTime.
        /// </summary>
        /// <param name="milliseconds">Milliseconds since epoch</param>
        /// <returns>DateTime</returns>
        public static DateTime FromEpochMilliseconds(ulong milliseconds)
        {
            return EpochDt.AddMilliseconds(milliseconds).ToUniversalTime();
        }

        /// <summary>
        /// Convert DateTime to milliseconds since epoch.
        /// </summary>
        /// <param name="dt">Input DateTime</param>
        /// <returns>Milliseconds since epoch</returns>
        public static ulong ToEpochMilliseconds(DateTime dt)
        {
            return (ulong)(dt.ToUniversalTime() - EpochDt.ToUniversalTime()).TotalMilliseconds;
        }

        /// <summary>
        /// Convert relative timestamp in milliseconds to absolute DateTime.
        /// DateTimeKind is UniversalTime.
        /// </summary>
        /// <param name="timestampMillis">Timestamp in milliseconds</param>
        /// <param name="reference">Reference timestamp</param>
        /// <returns>Absolute DateTime</returns>
        public static DateTime FromTimestampMilliseconds(ulong timestampMillis, DateTime reference)
        {
            return (reference.ToUniversalTime() + TimeSpan.FromMilliseconds(timestampMillis));
        }

        /// <summary>
        /// Convert DateTime to relative milliseconds timestamp.
        /// </summary>
        /// <param name="now">Current DateTime</param>
        /// <param name="reference">Reference DateTime</param>
        /// <returns></returns>
        public static ulong ToTimestampMilliseconds(DateTime now, DateTime reference)
        {
            return (ulong)(now.ToUniversalTime() - reference.ToUniversalTime()).TotalMilliseconds;
        }

        /// <summary>
        /// Convert relative timestamp in microseconds to absolute DateTime.
        /// DateTimeKind is UniversalTime.
        /// </summary>
        /// <param name="timestampMicros">Timestamp in microseconds</param>
        /// <param name="reference">Reference timestamp</param>
        /// <returns>Absolute DateTime</returns>
        public static DateTime FromTimestampMicroseconds(ulong timestampMicros, DateTime reference)
        {
            return (reference.ToUniversalTime() + TimeSpan.FromMilliseconds(timestampMicros / 1000));
        }

        /// <summary>
        /// Convert DateTime to relative microseconds timestamp.
        /// </summary>
        /// <param name="now">Current DateTime</param>
        /// <param name="reference">Reference DateTime</param>
        /// <returns></returns>
        public static ulong ToTimestampMicroseconds(DateTime now, DateTime reference)
        {
            return (ulong)(now.ToUniversalTime() - reference.ToUniversalTime()).TotalMilliseconds * 1000;
        }
    }
}
