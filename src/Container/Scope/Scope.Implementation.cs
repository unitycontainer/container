namespace Unity.Container
{
    public partial class ContainerScope
    {
        protected void ReplaceManager(ref Registry registry, RegistrationManager manager)
        {
            // TODO: Dispose manager
            registry.Manager = manager;
        
        }
    }
}
