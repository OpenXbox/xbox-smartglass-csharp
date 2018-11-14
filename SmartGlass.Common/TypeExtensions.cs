using System;

namespace SmartGlass.Common
{
    public static class TypeExtensions
    {
        public static bool IsAssignableTo(this Type type, Type c)
        {
            return c.IsAssignableFrom(type);
        }
    }
}