

using Unity.Builder;

namespace Unity.Policy
{
    /// <summary>
    /// Represents a builder policy for mapping build keys.
    /// </summary>
    public interface IBuildKeyMappingPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Maps the build key.
        /// </summary>
        /// <param name="buildKey">The build key to map.</param>
        /// <param name="context">Current build context. Used for contextual information
        /// if writing a more sophisticated mapping. This parameter can be null
        /// (called when getting container registrations).</param>
        /// <returns>The new build key.</returns>
        INamedType Map(INamedType buildKey, IBuilderContext context);

        /// <summary>
        /// Instructs engine to resolve type rather than build it
        /// </summary>
        bool RequireBuild { get; }
    }
}
