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

        internal static MethodInfo EnumerableMethod =
            typeof(EnumerableResolver).GetTypeInfo()
                                      .GetDeclaredMethod(nameof(EnumerableResolver.Resolver));

        internal static MethodInfo EnumerableFactory =
            typeof(EnumerableResolver).GetTypeInfo()
                                      .GetDeclaredMethod(nameof(EnumerableResolver.ResolverFactory));

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
                return ((EnumerableFactoryDelegate)
                    EnumerableFactory.MakeGenericMethod(typeArgument)
                                     .CreateDelegate(typeof(EnumerableFactoryDelegate)))();
            }
            else
            {
                return (ResolveDelegate<BuilderContext>)
                    EnumerableMethod.MakeGenericMethod(typeArgument)
                                    .CreateDelegate(typeof(ResolveDelegate<BuilderContext>));
            }
        };

        #endregion


        #region Implementation

        private static object Resolver<TElement>(ref BuilderContext context)
        {
            return ((UnityContainer)context.Container).ResolveEnumerable<TElement>(context.Resolve,
                                                                                   context.Name);
        }

        private static ResolveDelegate<BuilderContext> ResolverFactory<TElement>()
        {
            Type type = typeof(TElement).GetGenericTypeDefinition();
            return (ref BuilderContext c) => ((UnityContainer)c.Container).ResolveEnumerable<TElement>(c.Resolve, type, c.Name);
        }


        internal static object DiagnosticResolver<TElement>(ref BuilderContext context)
        {
            return ((UnityContainer)context.Container).ResolveEnumerable<TElement>(context.Resolve,
                                                                                   context.Name).ToArray();
        }

        internal static ResolveDelegate<BuilderContext> DiagnosticResolverFactory<TElement>()
        {
            Type type = typeof(TElement).GetGenericTypeDefinition();
            return (ref BuilderContext c) => ((UnityContainer)c.Container).ResolveEnumerable<TElement>(c.Resolve, type, c.Name).ToArray();
        }

        #endregion


        #region Nested Types

        private delegate ResolveDelegate<BuilderContext> EnumerableFactoryDelegate();

        #endregion
    }
}
