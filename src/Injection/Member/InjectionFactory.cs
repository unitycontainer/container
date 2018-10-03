using System;
using Unity.Delegates;
using Unity.Lifetime;
using Unity.Registration;

namespace Unity.Injection
{
    /// <summary>
    /// A class that lets you specify a factory method the container
    /// will use to create the object.
    /// </summary>
    /// <remarks>This factory allow using predefined <code>Func&lt;IUnityContainer, Type, string, object&gt;</code> to create types.</remarks>
    public class InjectionFactory : InjectionMember, IInjectionFactory
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
            : this((c, t, s) => factoryFunc(c))
        {
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


        #region IInjectionFactory

        /// <summary>
        /// Add policies to the <paramref name="policies"/> to configure the
        /// container to call this constructor with the appropriate parameter values.
        /// </summary>
        /// <param name="registeredType">Type of interface being registered. If no interface,
        /// this will be null. This parameter is ignored in this implementation.</param>
        /// <param name="mappedToType">Type of concrete type being registered.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public override void AddPolicies<TContext, TPolicyList>(Type registeredType, Type mappedToType, string name, ref TPolicyList policies)
        {
            if (null != mappedToType && mappedToType != registeredType)
                throw new InvalidOperationException(
                    "Registration where both MappedToType and InjectionFactory are set is not supported");

            var lifetime = policies.Get(registeredType, name, typeof(ILifetimePolicy));
            if (lifetime is PerResolveLifetimeManager)
            {
                policies.Set(registeredType, name, typeof(ResolveDelegate<TContext>),
                    (ResolveDelegate<TContext>)((ref TContext context) =>
                    {
                        var result = _factoryFunc(context.Container, context.Type, context.Name);
                        var perBuildLifetime = new InternalPerResolveLifetimeManager(result);
                        context.Set(context.Type, context.Name, typeof(ILifetimePolicy), perBuildLifetime);
                        return result;
                    }));
            }
            else
            {
                policies.Set(registeredType, name, typeof(ResolveDelegate<TContext>),
                    (ResolveDelegate<TContext>)((ref TContext context) =>
                        _factoryFunc(context.Container, context.Type, context.Name)));
            }
        }

        #endregion


        #region Nested Types

        internal sealed class InternalPerResolveLifetimeManager : PerResolveLifetimeManager
        {
            /// <summary>
            /// Construct a new <see cref="PerResolveLifetimeManager"/> object that stores the
            /// give value. This value will be returned by <see cref="LifetimeManager.GetValue"/>
            /// but is not stored in the lifetime manager, nor is the value disposed.
            /// This Lifetime manager is intended only for internal use, which is why the
            /// normal <see cref="LifetimeManager.SetValue"/> method is not used here.
            /// </summary>
            /// <param name="obj">InjectionParameterValue to store.</param>
            public InternalPerResolveLifetimeManager(object obj)
            {
                value = obj;
                InUse = true;
            }
        }

        #endregion
    }
}
