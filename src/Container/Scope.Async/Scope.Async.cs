using Unity.Lifetime;

namespace Unity.Scope
{
    public partial class ContainerScopeAsync : UnityContainer.ContainerScope
    {
        internal ContainerScopeAsync(UnityContainer container) 
            : base(container)
        {
            // Register IUnityContainerAsync Interface
            Add(typeof(IUnityContainerAsync), _registry[0].Manager);
        }

        public override UnityContainer.ContainerScope CreateChildScope(UnityContainer container)
            => new ContainerScopeAsync(container);
    }
}
