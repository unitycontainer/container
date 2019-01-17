namespace Unity.Resolution
{
    public delegate object ResolveDelegate<TContext>(ref TContext context)
        where TContext : IResolveContext;


    /// <summary>
    /// A strategy that is used at build plan execution time
    /// to resolve a dependent value.
    /// </summary>
    public interface IResolve 
    {
        /// <summary>
        /// GetOrDefault the value
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>The value for the dependency.</returns>
        object Resolve<TContext>(ref TContext context) 
            where TContext : IResolveContext;
    }
}

