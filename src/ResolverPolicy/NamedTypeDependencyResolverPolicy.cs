using System;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.ResolverPolicy
{
    /// <summary>
    /// An implementation of <see cref="IResolver"/> that stores a
    /// type and name, and at resolution time puts them together into a
    /// <see cref="NamedTypeBuildKey"/>.
    /// </summary>
    public class NamedTypeDependencyResolverPolicy : IResolver
    {
        /// <summary>
        /// Create an instance of <see cref="NamedTypeDependencyResolverPolicy"/>
        /// with the given type and name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name (may be null).</param>
        public NamedTypeDependencyResolverPolicy(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        /// <summary>
        /// Resolve the value for a dependency.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>The value for the dependency.</returns>
        public object Resolve<TContext>(ref TContext context)
            where TContext : IResolveContext
        {
            return context.Resolve(Type, Name);
        }

        /// <summary>
        /// The type that this resolver resolves.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// The name that this resolver resolves.
        /// </summary>
        public string Name { get; }
    }
}
