using Unity.Builder;
using Unity.Resolution;

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
        /// <returns>The value for the dependency.</returns>
        object Resolve(params ResolverOverride[] args);
    }
}
