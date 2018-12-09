using System;
using Unity.Builder;
using Unity.Resolution;

namespace Unity.Policy.Mapping
{
    /// <summary>
    /// Represents a builder policy for mapping build keys.
    /// </summary>
    public class BuildKeyMappingPolicy : NamedTypeBase, IBuildKeyMappingPolicy
    {
        #region Constructors

        /// <summary>
        /// Initialize a new instance of the <see cref="BuildKeyMappingPolicy"/> with the new build key.
        /// </summary>
        /// <param name="newBuildKey">The new build key.</param>
        public BuildKeyMappingPolicy(INamedType newBuildKey)
            : base(newBuildKey.Type, newBuildKey.Name)
        {
        }

        public BuildKeyMappingPolicy(Type type, string name, bool build)
            : base(type, name)
        {
            RequireBuild = build;
        }

        #endregion


        #region IBuildKeyMappingPolicy


        /// <summary>
        /// Maps the build key.
        /// </summary>
        /// <param name="buildKey">The build key to map.</param>
        /// <param name="context">Current build context. Used for contextual information
        /// if writing a more sophisticated mapping, unused in this implementation.</param>
        /// <returns>The new build key.</returns>
        public INamedType Map<TContext>(INamedType buildKey, ref TContext context) 
            where TContext : IResolveContext
        {
            return this;
        }

        /// <summary>
        /// Instructs engine to resolve type rather than build it
        /// </summary>
        public bool RequireBuild { get; } = true;

        #endregion
    }
}
