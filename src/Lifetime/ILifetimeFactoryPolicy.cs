

using System;
using Unity.Builder;
using Unity.Policy;

namespace Unity.Lifetime
{
    /// <summary>
    /// A builder policy used to create lifetime policy instances.
    /// Used by the LifetimeStrategy when instantiating open
    /// generic types.
    /// </summary>
    public interface ILifetimeFactoryPolicy 
    {
        /// <summary>
        /// Create a new instance of <see cref="ILifetimePolicy"/>.
        /// </summary>
        /// <returns>The new instance.</returns>
        ILifetimePolicy CreateLifetimePolicy();

        /// <summary>
        /// The type of Lifetime manager that will be created by this factory.
        /// </summary>
        Type LifetimeType { get; }
    }
}
