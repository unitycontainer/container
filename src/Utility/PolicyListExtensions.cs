using System;
using System.Reflection;

namespace Unity.Policy
{
    public static class LegacyPolicyListUtilityExtensions
    {
        internal static TPolicyInterface GetPolicy<TPolicyInterface>(this IPolicyList list, Type type, string name)
        {
            var info = type.GetTypeInfo();

            return (TPolicyInterface)(
                list.Get(type, name, typeof(TPolicyInterface)) ?? (
                    info.IsGenericType ? list.Get(type.GetGenericTypeDefinition(), name, typeof(TPolicyInterface)) ??
                                         list.Get(null, null, typeof(TPolicyInterface))
                        : list.Get(null, null, typeof(TPolicyInterface))));
        }
    }
}
