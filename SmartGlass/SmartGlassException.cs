using System;

namespace DarkId.SmartGlass
{
    public class SmartGlassException : Exception
    {
        public SmartGlassException(string message) : base(message)
        {
        }

        public SmartGlassException(string message, int result) : base(message)
        {
            HResult = result;
        }

        public SmartGlassException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}