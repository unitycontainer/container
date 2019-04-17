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
        #region Delegates

        private delegate object EnumerableResolverDelegate(Func<Type, string, object> resolve,
                                                           Func<Type, string, InternalRegistration, object> resolveRegistration,
                                                           string name, UnityContainer unity);
        #endregion


        #region Fields

        private static readonly MethodInfo EnumerableMethod =
            typeof(UnityContainer).GetTypeInfo()
                                  .GetDeclaredMethod(nameof(UnityContainer.ResolveEnumerable));

        private static readonly MethodInfo EnumerableGeneric =
            typeof(UnityContainer).GetTypeInfo()
                                  .GetDeclaredMethod(nameof(UnityContainer.ResolveEnumerableGeneric));

        #endregion


        #region ResolveDelegateFactory

        public static ResolveDelegateFactory Factory = (ref BuilderContext context) =>
        {
            EnumerableResolverDelegate method;

#if NETSTANDARD1_0 || NETCOREAPP1_0 || NET40
            var typeArgument = context.Type.GetTypeInfo().GenericTypeArguments.First();
            if (typeArgument.GetTypeInfo().IsGenericType)
#else
            var typeArgument = context.Type.GenericTypeArguments.First();
            if (typeArgument.IsGenericType)
#endif
            {
                method = (EnumerableResolverDelegate)EnumerableGeneric
                    .MakeGenericMethod(typeArgument)
                    .CreateDelegate(typeof(EnumerableResolverDelegate));
            }
            else
            {

                method = (EnumerableResolverDelegate)EnumerableMethod
                    .MakeGenericMethod(typeArgument)
                    .CreateDelegate(typeof(EnumerableResolverDelegate));
            }

            return (ref BuilderContext c) =>
            {
                return method(c.Resolve, c.Resolve, c.Name, (UnityContainer)c.Container);
            };
        };

        #endregion
    }
}
