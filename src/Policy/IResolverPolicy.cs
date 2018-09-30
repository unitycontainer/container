using Unity.Build;

namespace Unity.Policy
{
    /// <summary>
    /// A strategy that is used at build plan execution time
    /// to resolve a dependent value.
    /// </summary>
    public interface IResolverPolicy 
    {
        /// <summary>
        /// GetOrDefault the value
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>The value for the dependency.</returns>
        object Resolve<TContext>(ref TContext context) 
            where TContext : IBuildContext;
    }
}
