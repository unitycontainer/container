

using Unity.Builder;
using Unity.Policy;

namespace Unity.ResolverPolicy
{
    /// <summary>
    /// A <see cref="IResolverPolicy"/> implementation that returns
    /// the value set in the constructor.
    /// </summary>
    public class LiteralValueDependencyResolverPolicy : IResolverPolicy
    {
        private readonly object _dependencyValue;

        /// <summary>
        /// Create a new instance of <see cref="LiteralValueDependencyResolverPolicy"/>
        /// which will return the given value when resolved.
        /// </summary>
        /// <param name="dependencyValue">The value to return.</param>
        public LiteralValueDependencyResolverPolicy(object dependencyValue)
        {
            _dependencyValue = dependencyValue;
        }

        /// <summary>
        /// GetOrDefault the value for a dependency.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>The value for the dependency.</returns>
        public object Resolve<TBuilderContext>(ref TBuilderContext context)
            where TBuilderContext : IBuilderContext
        {
            return _dependencyValue;
        }
    }
}
