using Unity.Builder;

namespace Unity.Policy
{
    /// <summary>
    /// A strategy that resolves a value.
    /// </summary>
    public interface IResolverPolicy : IBuilderPolicy
    {
        /// <summary>
        /// GetOrDefault the value
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>The value for the dependency.</returns>
        object Resolve(IBuilderContext context);
    }
}
