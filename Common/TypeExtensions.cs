using System;

namespace SmartGlass.Common
{
    static class TypeExtensions
    {
        public static bool IsAssignableTo(this Type type, Type c)
        {
            return c.IsAssignableFrom(type);
        }
    }
}