using System;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Factories
{
    internal class DeferredFuncResolverFactory
    {
        private static readonly MethodInfo DeferredResolveMethodInfo
            = typeof(DeferredFuncResolverFactory).GetTypeInfo()
                                                 .GetDeclaredMethod(nameof(DeferredResolve));
        public static ResolveDelegate<BuilderContext> DeferredResolveDelegateFactory(ref BuilderContext context)
        {
            var typeToBuild = context.Type.GetTypeInfo().GenericTypeArguments[0];
            var factoryMethod = DeferredResolveMethodInfo.MakeGenericMethod(typeToBuild);

            return (ResolveDelegate<BuilderContext>)factoryMethod.CreateDelegate(typeof(ResolveDelegate<BuilderContext>)); 
        }

        private static Func<T> DeferredResolve<T>(ref BuilderContext context)
        {
            var nameToBuild = context.Name;
            var container = context.Container;

            return () => (T)container.Resolve<T>(nameToBuild);
        }
    }
}
