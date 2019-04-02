using System;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.Injection
{
    /// <inheritdoc />
    /// <summary>
    /// A class that lets you specify a factory method the container
    /// will use to create the object.
    /// </summary>
    /// <remarks>This factory allow using predefined <code>Func&lt;IUnityContainer, Type, string, object&gt;</code> to create types.</remarks>
    [Obsolete("InjectionFactory has been deprecated and will be removed in next release. Please use IUnityContainer.RegisterFactory(...) method instead.", false)]
    public class InjectionFactory : InjectionMember
    {
        #region Fields

        private readonly Func<IUnityContainer, Type, string, object> _factoryFunc;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a new instance of <see cref="InjectionFactory"/> with
        /// the given factory function.
        /// </summary>
        /// <param name="factoryFunc">Factory function.</param>
        public InjectionFactory(Func<IUnityContainer, object> factoryFunc)
        {
            if (null == factoryFunc) throw new ArgumentNullException(nameof(factoryFunc));
            _factoryFunc = (c, t, s) => factoryFunc(c);
        }

        /// <summary>
        /// Create a new instance of <see cref="InjectionFactory"/> with
        /// the given factory function.
        /// </summary>
        /// <param name="factoryFunc">Factory function.</param>
        public InjectionFactory(Func<IUnityContainer, Type, string, object> factoryFunc)
        {
            _factoryFunc = factoryFunc ?? throw new ArgumentNullException(nameof(factoryFunc));
        }

        #endregion


        #region InjectionMember

        /// <summary>
        /// Add policies to the <paramref name="policies"/> to configure the
        /// container to call this constructor with the appropriate parameter values.
        /// </summary>
        /// <param name="registeredType">Type of interface being registered. If no interface,
        /// this will be null. This parameter is ignored in this implementation.</param>
        /// <param name="mappedToType">Type of concrete type being registered.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public override void AddPolicies<TContext, TPolicySet>(Type registeredType, Type mappedToType, string name, ref TPolicySet policies)
        {
            // Verify
            if (null != mappedToType && mappedToType != registeredType)
                throw new InvalidOperationException(
                    "Registration where both MappedToType and InjectionFactory are set is not supported");

            // Check if Per Resolve lifetime is required
            var lifetime = policies.Get(typeof(LifetimeManager));
            if (lifetime is PerResolveLifetimeManager)
            {
                policies.Set(typeof(ResolveDelegate<TContext>), CreatePerResolveLegacyPolicy());
            }
            else
            {
                policies.Set(typeof(ResolveDelegate<TContext>), CreateLegacyPolicy());
            }

            // Factory methods

            ResolveDelegate<TContext> CreateLegacyPolicy()
            {
                return (ref TContext c) => _factoryFunc(c.Container, c.Type, c.Name);
            }

            ResolveDelegate<TContext> CreatePerResolveLegacyPolicy() 
            {
                return (ref TContext context) =>
                {
                    var result = _factoryFunc(context.Container, context.Type, context.Name);
                    var perBuildLifetime = new InternalPerResolveLifetimeManager(result);

                    context.Set(context.Type, context.Name, typeof(LifetimeManager), perBuildLifetime);
                    return result;
                }; 
            }
        }

        #endregion


        #region Nested Types

        internal sealed class InternalPerResolveLifetimeManager : PerResolveLifetimeManager
        {
            public InternalPerResolveLifetimeManager(object obj)
            {
                value = obj;
                InUse = true;
            }
        }

        #endregion
    }
}
