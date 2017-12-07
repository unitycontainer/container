using Unity.Container;
using Unity.Resolution;

namespace Unity.Policy
{
    /// <summary>
    /// A strategy that resolves a value.
    /// </summary>
    public interface IResolverPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Resolves instance
        /// </summary>
        /// <param name="context">Current container context.</param>
        /// <param name="resolverOverrides">Any overrides for the resolve call.</param>
        /// <returns>Returns resolved value</returns>
        object Resolve(IContainerContext context, ResolverOverride[] resolverOverrides = null);
    }
}
