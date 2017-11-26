// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity.Extension;

namespace Unity.Lifetime
{
    /// <summary>
    /// An implementation of <see cref="ILifetimeFactoryPolicy"/> that
    /// creates instances of the type of the given Lifetime Manager
    /// by resolving them through the container.
    /// </summary>
    public class LifetimeManagerFactory : ILifetimeFactoryPolicy
    {
        private readonly ExtensionContext _containerContext;

        /// <summary>
        /// Create a new <see cref="LifetimeManagerFactory"/> that will
        /// return instances of the given type, creating them by
        /// resolving through the container.
        /// </summary>
        /// <param name="containerContext">Container to resolve with.</param>
        /// <param name="lifetimeType">Type of LifetimeManager to create.</param>
        public LifetimeManagerFactory(ExtensionContext containerContext, Type lifetimeType)
        {
            _containerContext = containerContext;
            LifetimeType = lifetimeType;
        }

        /// <summary>
        /// Create a new instance of <see cref="ILifetimePolicy"/>.
        /// </summary>
        /// <returns>The new instance.</returns>
        public ILifetimePolicy CreateLifetimePolicy()
        {
            var lifetime = typeof(TransientLifetimeManager) == LifetimeType
                ? new TransientLifetimeManager()
                : (LifetimeManager)_containerContext.Container.Resolve(LifetimeType, null);

            if (lifetime is IDisposable)
            {
                _containerContext.Lifetime.Add(lifetime);
            }
            lifetime.InUse = true;
            return lifetime;
        }

        /// <summary>
        /// The type of Lifetime manager that will be created by this factory.
        /// </summary>
        public Type LifetimeType { get; }
    }
}
