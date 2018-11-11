using System;

namespace DarkId.SmartGlass.Common
{
    static class TypeExtensions
    {
        public static bool IsAssignableTo(this Type type, Type c)
        {
            return c.IsAssignableFrom(type);
        }
    }
}