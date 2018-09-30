using Unity.Build;

namespace Unity.Policy
{
    /// <summary>
    /// A <see cref="IBuilderPolicy"/> that, when implemented,
    /// will determine which constructor to call from the build plan.
    /// </summary>
    public interface IConstructorSelectorPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Choose the constructor to call for the given type.
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>The chosen constructor.</returns>
        object SelectConstructor<TContext>(ref TContext context)
            where TContext : IBuildContext;
    }
}
