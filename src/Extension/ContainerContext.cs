using System;
using Unity.Lifetime;
using Unity.Policy;

namespace Unity.Extension
{
    /// <summary>
    /// Context of a local scope
    /// </summary>
    public abstract class ContainerContext : IPolicyList
    {
        #region Scope Lifetime Container

        public abstract ILifetimeContainer LifetimeContainer { get; }

        #endregion


        #region Default Lifetime Managers

        /// <summary>
        /// Default implicit lifetime of type registrations 
        /// </summary>
        public abstract LifetimeManager DefaultTypeLifetime { get; set; }

        /// <summary>
        /// Default implicit lifetime of factory registrations 
        /// </summary>
        public abstract LifetimeManager DefaultFactoryLifetime { get; set; }

        /// <summary>
        /// Default implicit lifetime of instance registrations 
        /// </summary>
        public abstract LifetimeManager DefaultInstanceLifetime { get; set; }

        #endregion


        #region IPolicyList

        /// <inheritdoc />
        public abstract void Clear(Type? type, string? name, Type policyInterface);

        /// <inheritdoc />
        public abstract object? Get(Type type, Type policyInterface);

        /// <inheritdoc />
        public abstract object? Get(Type? type, string? name, Type policyInterface);

        /// <inheritdoc />
        public abstract void Set(Type type, Type policyInterface, object policy);

        /// <inheritdoc />
        public abstract void Set(Type? type, string? name, Type policyInterface, object policy);

        #endregion
    }
}
