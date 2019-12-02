using System;
using System.Linq;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Factories
{
    public interface IRegex<TType>
    {
    }

    public class RegExResolver
    {
        #region Fields

        private static readonly MethodInfo EnumerableMethod =
            typeof(RegExResolver).GetTypeInfo()
                                 .GetDeclaredMethod(nameof(RegExResolver.Resolver))!;

        private static readonly MethodInfo EnumerableFactory =
            typeof(RegExResolver).GetTypeInfo()
                                 .GetDeclaredMethod(nameof(RegExResolver.ResolverFactory))!;

        #endregion


        #region ResolveDelegateFactory

        public static TypeFactoryDelegate Factory = (Type type, UnityContainer container) =>
        {

#if NETSTANDARD1_0 || NETCOREAPP1_0 || NET40
            var typeArgument = type.GetTypeInfo().GenericTypeArguments.First();
            if (typeArgument.GetTypeInfo().IsGenericType)
#else
            var typeArgument = type.GenericTypeArguments.First();
            if (typeArgument.IsGenericType)
#endif
            {
                return ((EnumerableFactoryDelegate)
                    EnumerableFactory.MakeGenericMethod(typeArgument)
                                     .CreateDelegate(typeof(EnumerableFactoryDelegate)))();
            }
            else
            {
                return (ResolveDelegate<PipelineContext>)
                    EnumerableMethod.MakeGenericMethod(typeArgument)
                                    .CreateDelegate(typeof(ResolveDelegate<PipelineContext>));
            }
        };

        #endregion


        #region Implementation

        private static object Resolver<TElement>(ref PipelineContext context)
        {
            return ((UnityContainer)context.Container).ResolveEnumerable<TElement>(context.Resolve,
                                                                                   context.Name);
        }

        private static ResolveDelegate<PipelineContext> ResolverFactory<TElement>()
        {
            Type type = typeof(TElement).GetGenericTypeDefinition();
            return (ref PipelineContext c) => ((UnityContainer)c.Container).ResolveEnumerable<TElement>(c.Resolve, type, c.Name);
        }

        #endregion


        #region Nested Types

        private delegate ResolveDelegate<PipelineContext> EnumerableFactoryDelegate();

        #endregion
    }
}
