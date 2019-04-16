using System;
using System.Reflection;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.Factories
{
    /// <summary>
    /// An Resolver Delegate Factory implementation
    /// that constructs a build plan for creating <see cref="Lazy{T}"/> objects.
    /// </summary>
    public class LazyResolver 
    {
        #region Fields

        private static readonly MethodInfo ImplementationMethod =
            typeof(LazyResolver).GetTypeInfo()
                                       .GetDeclaredMethod(nameof(ResolverImplementation));

        #endregion


        #region ResolveDelegateFactory

        public static ResolveDelegateFactory Factory = (ref BuilderContext context) =>
        {
            var itemType = context.Type.GetTypeInfo().GenericTypeArguments[0];
            var lazyMethod = ImplementationMethod.MakeGenericMethod(itemType);

            return (ResolveDelegate<BuilderContext>)lazyMethod.CreateDelegate(typeof(ResolveDelegate<BuilderContext>));
        };

        #endregion


        #region Implementation

        private static object ResolverImplementation<T>(ref BuilderContext context)
        {
            var container = context.Container;
            var name = context.Name;

            context.Existing = new Lazy<T>(() => container.Resolve<T>(name));

            var lifetime = context.Get(typeof(LifetimeManager));
            if (lifetime is PerResolveLifetimeManager)
            {
                var perBuildLifetime = new InternalPerResolveLifetimeManager(context.Existing);
                context.Set(typeof(LifetimeManager), perBuildLifetime);
            }

            return context.Existing;
        }

        #endregion
    }
}
