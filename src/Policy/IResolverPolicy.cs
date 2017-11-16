using Unity.Builder;

namespace Unity.Policy
{
    /// <summary>
    /// A strategy that resolves a value.
    /// </summary>
    public interface IResolver_Policy : IBuilderPolicy
    {
        /// <summary>
        /// GetOrDefault the value
        /// </summary>
        /// <returns>The value for the dependency.</returns>
        object Resolve();
    }
}
