using System;
using System.Linq;
using System.Reflection;


namespace Unity
{
    /// <summary>
    /// Provides compatibility layer for NET4.0.
    /// </summary>
    internal static class Compatibility_Net4_0
    {
        public static Attribute? GetCustomAttribute(this MemberInfo info, Type type)
        {
            return info.GetCustomAttributes(true)
                       .Cast<Attribute>()
                       .Where(a => type.GetTypeInfo().IsAssignableFrom(a.GetType().GetTypeInfo()))
                       .FirstOrDefault();
        }

        public static bool IsDefined(this MemberInfo member, Type type)
        {
            return member.IsDefined(type, true);
        }
    }
}
