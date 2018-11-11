using System;
using System.Globalization;
using System.Linq;

namespace SmartGlass.Cli
{
    internal static class BinaryExtensions
    {
        public static byte[] HexToBytes(this string hex)
        {
            return hex.Split(2).Select(c =>
                byte.Parse(new string(c.ToArray()), NumberStyles.HexNumber)).ToArray();
        }

        public static string ToHex(this byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}