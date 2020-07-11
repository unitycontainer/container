namespace Unity.Container
{
    public partial class ContainerScope
    {
        protected void ReplaceManager(ref Registry registry, RegistrationManager manager)
        {
            // TODO: Dispose manager
            registry.Manager = manager;
        
        }

        public override int GetHashCode()
        {
            int hash = HASH_CODE_SEED;
            var scope = this;

            do {

                hash = hash * scope._level + scope._version;
            
            } while (null != (scope = scope.Parent));

            return hash;
        }
    }
}
