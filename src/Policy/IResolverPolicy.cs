using Unity.Builder;

namespace Unity.Policy
{
    /// <summary>
    /// A strategy that is used at build plan execution time
    /// to resolve a dependent value.
    /// </summary>
    public interface IResolverPolicy : IBuilderPolicy
    {
        /// <summary>
        /// GetOrDefault the value
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>The value for the dependency.</returns>
        object Resolve<TBuilderContext>(ref TBuilderContext context) 
            where TBuilderContext : IBuilderContext;
    }
}
