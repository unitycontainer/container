using System;
using System.Collections.Generic;
using Unity.Lifetime;
using Unity.Policy;

namespace Unity.Extension
{
    /// <summary>
    /// Context of a local scope
    /// </summary>
    public abstract class ContainerContext
    {
        /// <summary>
        /// Name of the container
        /// </summary>
        public abstract string? Name { get; set; }

        /// <summary>
        /// The policies this container uses.
        /// </summary>
        /// <remarks>The <see cref="IPolicyList"/> the that container uses to build objects.</remarks>
        public abstract IPolicyList Policies { get; }

        /// <summary>
        /// List of disposable objects this container holds
        /// </summary>
        public abstract ICollection<IDisposable> Lifetime { get; }


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
    }
}
