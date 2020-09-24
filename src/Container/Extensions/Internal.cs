using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity
{
    public static class InternalExtensions
    {
        private static readonly TypeInfo DelegateType = typeof(Delegate).GetTypeInfo();


        public static bool CanResolve(this UnityContainer container,  Type type, string? name)
        {
#if NETSTANDARD1_6 || NETCOREAPP1_0
            var info = type.GetTypeInfo();
#else
            var info = type;
#endif
            if (info.IsClass)
            {
                // Array could be either registered or Type can be resolved
                if (type.IsArray)
                {
                    return container.IsRegistered(type, name) || container.CanResolve(type.GetElementType()!, name);
                }

                // Type must be registered if:
                // - String
                // - Enumeration
                // - Primitive
                // - Abstract
                // - Interface
                // - No accessible constructor
                if (DelegateType.IsAssignableFrom(info) ||
                    typeof(string) == type || info.IsEnum || info.IsPrimitive || info.IsAbstract
#if NETSTANDARD1_6 || NETCOREAPP1_0
                    || !info.DeclaredConstructors.Any(c => !c.IsFamily && !c.IsPrivate))
#else
                    || !type.GetTypeInfo().DeclaredConstructors.Any(c => !c.IsFamily && !c.IsPrivate))
#endif
                    return container.IsRegistered(type, name);

                return true;
            }

            // Can resolve if IEnumerable or factory is registered
            if (info.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                if (genericType == typeof(IEnumerable<>) || container.IsRegistered(genericType, name))
                {
                    return true;
                }
            }

            // Check if Type is registered
            return container.IsRegistered(type, name);
        }
    }
}
