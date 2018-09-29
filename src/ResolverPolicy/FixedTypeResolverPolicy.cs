using System;
using Unity.Builder;
using Unity.Policy;

namespace Unity.ResolverPolicy
{
    /// <summary>
    /// An implementation of <see cref="IResolverPolicy"/> that
    /// calls back into the build chain to build up the dependency, passing
    /// a type given at compile time as its build key.
    /// </summary>
    public class FixedTypeResolverPolicy : IResolverPolicy
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
        public object Resolve<TBuilderContext>(ref TBuilderContext context)
            where TBuilderContext : IBuilderContext
        {
            return context.NewBuildUp(_keyToBuild.Type, _keyToBuild.Name);
        }

        #endregion
    }
}
