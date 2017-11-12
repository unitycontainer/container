using Unity.Policy;

namespace Unity.Lifetime
{
    public interface IHierarchicalLifetimePolicy : ILifetimePolicy
    {
        /// <summary>
        /// Creates controller for current scope
        /// </summary>
        /// <returns>IScopeLifetimePolicy</returns>
        IBuilderPolicy CreateScope();
    }
}
