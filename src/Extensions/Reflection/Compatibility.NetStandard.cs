using System;
using System.Reflection;

namespace Unity
{
#if NETSTANDARD1_6 || NETSTANDARD1_0
    internal static class Delegate
    {
        public static object CreateDelegate(Type type, object target, MethodInfo info)
            => info.CreateDelegate(type, target);
    }
#endif

}
