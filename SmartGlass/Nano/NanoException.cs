using System;

namespace SmartGlass.Nano
{
    public class NanoException : Exception
    {
        public NanoException(string message) : base(message)
        {
        }

        public NanoException(string message, int result) : base(message)
        {
            HResult = result;
        }

        public NanoException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}