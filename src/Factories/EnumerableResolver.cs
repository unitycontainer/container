using System;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;
using Unity.Registration;

namespace Unity.Factories
{
    public class EnumerableResolver
    {
        #region Fields

        private static readonly MethodInfo EnumerableMethod =
            typeof(UnityContainer).GetTypeInfo()
                                  .GetDeclaredMethod(nameof(UnityContainer.ResolveEnumerable));

        private static readonly MethodInfo GenericEnumerable =
            typeof(UnityContainer).GetTypeInfo()
                                  .GetDeclaredMethod(nameof(UnityContainer.ResolveGenericEnumerable));

        #endregion


        #region ResolveDelegateFactory

        public static ResolveDelegateFactory Factory = (ref BuilderContext context) =>
        {
            Delegate method;

#if NETSTANDARD1_0 || NETCOREAPP1_0 || NET40
            var typeArgument = context.Type.GetTypeInfo().GenericTypeArguments.First();
            if (typeArgument.GetTypeInfo().IsGenericType)
#else
            var typeArgument = context.Type.GenericTypeArguments.First();
            if (typeArgument.IsGenericType)
#endif
            {
                // Generic closures
                method = GenericEnumerable.MakeGenericMethod(typeArgument)
                                          .CreateDelegate(typeof(ResolveGenericEnumerableDelegate));

                Type definition = typeArgument.GetGenericTypeDefinition();
                int hashCode = typeArgument.GetHashCode() & UnityContainer.HashMask;
                int hashGeneric = definition.GetHashCode() & UnityContainer.HashMask;

                return (ref BuilderContext c) =>
                {
                    return ((ResolveGenericEnumerableDelegate)method)(c.Resolve, c.Resolve, hashCode, hashGeneric, definition, c.Name, (UnityContainer)c.Container);
                };
            }
            else
            {
                // Closures
                method = EnumerableMethod.MakeGenericMethod(typeArgument)
                                         .CreateDelegate(typeof(ResolveEnumerableDelegate));

                int hashCode = typeArgument.GetHashCode() & UnityContainer.HashMask;

                return (ref BuilderContext c) =>
                {
                    return ((ResolveEnumerableDelegate)method)(c.Resolve, c.Resolve, hashCode, c.Name, (UnityContainer)c.Container);
                };
            }
        };

        #endregion


        #region Nested Types

        private delegate object ResolveEnumerableDelegate(Func<Type, string, object> resolve,
                                                          Func<Type, string, InternalRegistration, object> resolveRegistration,
                                                          int hashCode, string name, UnityContainer unity);

        private delegate object ResolveGenericEnumerableDelegate(Func<Type, string, object> resolve,
                                                                 Func<Type, string, InternalRegistration, object> resolveRegistration,
                                                                 int hashCode, int hashGeneric, Type typeDefinition,
                                                                 string name, UnityContainer unity);
        #endregion
    }
}
