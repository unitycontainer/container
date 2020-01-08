using System;
using System.Linq;
using System.Reflection;


namespace Unity
{
    /// <summary>
    /// Provides compatibility layer for NET Core 1.0.
    /// </summary>
    internal static class Compatibility_NetCoreApp_1_0
    {
        public static MethodInfo GetMethod(this Type type, string name)
        {
            return type.GetTypeInfo().DeclaredMethods.First(m => m.Name == name);
        }
    }
}
