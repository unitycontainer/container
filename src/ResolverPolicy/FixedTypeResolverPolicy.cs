using System;
using Unity.Builder;
using Unity.Policy;
using Unity.Resolution;

namespace Unity.ResolverPolicy
{
    /// <summary>
    /// An implementation of <see cref="IResolver"/> that
    /// calls back into the build chain to build up the dependency, passing
    /// a type given at compile time as its build key.
    /// </summary>
    public class FixedTypeResolverPolicy : IResolver
    {
        private readonly NamedTypeBuildKey _keyToBuild;

        /// <summary>
        /// Create a new instance storing the given type.
        /// </summary>
        /// <param name="typeToBuild">Type to resolve.</param>
        public FixedTypeResolverPolicy(Type typeToBuild)
        {
            _keyToBuild = new NamedTypeBuildKey(typeToBuild);
        }

        #region IResolverPolicy Members

        /// <summary>
        /// GetOrDefault the value for a dependency.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>The value for the dependency.</returns>
        public object Resolve<TContext>(ref TContext context)
            where TContext : IResolveContext
        {
            return context.Resolve(_keyToBuild.Type, _keyToBuild.Name);
        }

        #endregion
    }
}
