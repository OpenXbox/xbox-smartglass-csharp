using System;

namespace SmartGlass.Common
{
    /// <summary>
    /// Convert to exception.
    /// </summary>
    public interface IConvertToException
    {
        Exception ToException();
    }
}
