using System;

namespace SmartGlass.Nano
{
    public class NanoPackingException : Exception
    {
        public NanoPackingException(string message) : base(message)
        {
        }

        public NanoPackingException(string message, int result) : base(message)
        {
            HResult = result;
        }

        public NanoPackingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}