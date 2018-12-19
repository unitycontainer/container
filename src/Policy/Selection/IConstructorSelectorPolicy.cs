using Unity.Builder;

namespace Unity.Policy
{
    /// <summary>
    /// A policy that, when implemented,
    /// will determine which constructor to call from the build plan.
    /// </summary>
    public interface IConstructorSelectorPolicy
    {
        /// <summary>
        /// Choose the constructor to call for the given type.
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>The chosen constructor.</returns>
        object SelectConstructor(ref BuilderContext context);
    }
}
