namespace Unity.Lifetime
{
    /// <summary>
    /// Internal container lifetime manager. 
    /// This manager distinguishes internal registration from user mode registration.
    /// </summary>
    /// <remarks>
    /// Works like the ExternallyControlledLifetimeManager, but uses 
    /// regular instead of weak references
    /// </remarks>
    internal class ContainerLifetimeManager : LifetimeManager, IInstanceLifetimeManager
    {
        public override object GetValue(ILifetimeContainer container = null)
        {
            return container.Container;
        }

        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return new ContainerLifetimeManager();
        }

        public override bool InUse { get => false; set => base.InUse = false; }
    }
}
