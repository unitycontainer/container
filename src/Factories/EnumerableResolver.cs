using System;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Factories
{
    public class EnumerableResolver
    {
        #region Fields

        private static readonly MethodInfo EnumerableMethod =
            typeof(EnumerableResolver).GetTypeInfo()
                                      .GetDeclaredMethod(nameof(EnumerableResolver.ResolveEnumerable));

        private static readonly MethodInfo GenericEnumerable =
            typeof(EnumerableResolver).GetTypeInfo()
                                      .GetDeclaredMethod(nameof(EnumerableResolver.ResolveGeneric));

        #endregion


        #region ResolveDelegateFactory

        public static ResolveDelegateFactory Factory = (ref BuilderContext context) =>
        {

#if NETSTANDARD1_0 || NETCOREAPP1_0 || NET40
            var typeArgument = context.Type.GetTypeInfo().GenericTypeArguments.First();
            if (typeArgument.GetTypeInfo().IsGenericType)
#else
            var typeArgument = context.Type.GenericTypeArguments.First();
            if (typeArgument.IsGenericType)
#endif
            {
                var method = (ResolveEnumerableDelegate)
                    GenericEnumerable.MakeGenericMethod(typeArgument)
                                     .CreateDelegate(typeof(ResolveEnumerableDelegate));

                Type type = typeArgument.GetGenericTypeDefinition();

                return (ref BuilderContext c) => method(ref c, type);
            }
            else
            {
                return (ResolveDelegate<BuilderContext>)EnumerableMethod.MakeGenericMethod(typeArgument)
                                                                        .CreateDelegate(typeof(ResolveDelegate<BuilderContext>));
            }
        };

        #endregion


        #region Implementation

        private static object ResolveEnumerable<TElement>(ref BuilderContext context)
        {
            return ((UnityContainer)context.Container).ResolveEnumerable<TElement>(context.Resolve,
                                                                                   context.Resolve,
                                                                                   context.Name);
        }

        private static object ResolveGeneric<TElement>(ref BuilderContext context, Type type)
        {
            return ((UnityContainer)context.Container).ResolveEnumerable<TElement>(context.Resolve,
                                                                                   context.Resolve,
                                                                                   type, context.Name);
        }

        #endregion


        #region Nested Types

        private delegate object ResolveEnumerableDelegate(ref BuilderContext context, Type type);

        #endregion
    }
}
