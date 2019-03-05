using System;

namespace SmartGlass.Common
{
    /// <summary>
    /// Type extensions.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Checks if one object is assignable to another.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if object is assignable to provided type, <c>false</c> otherwise.
        /// </returns>
        /// <param name="type">Type to assigning.</param>
        /// <param name="c">Object to check.</param>
        public static bool IsAssignableTo(this Type type, Type c)
        {
            return c.IsAssignableFrom(type);
        }
    }
}
