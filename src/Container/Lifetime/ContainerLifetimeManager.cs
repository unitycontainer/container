using Unity.Builder;
using Unity.Lifetime;

namespace Unity.Container.Lifetime
{
    /// <summary>
    /// Internal container lifetime manager. 
    /// This manager distinguishes internal registration from user mode registration.
    /// </summary>
    /// <remarks>
    /// Works like the ExternallyControlledLifetimeManager, but uses 
    /// regular instead of weak references
    /// </remarks>
    internal class ContainerLifetimeManager : LifetimeManager
    {
        private object _value;

        public override object GetValue(ILifetimeContainer container = null)
        {
            return _value;
        }

        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
            _value = newValue;
        }

        public override void RemoveValue(ILifetimeContainer container = null)
        {
        }

        public object Resolve(IBuilderContext _)
        {
            return _value;
        }

        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return new ContainerLifetimeManager();
        }
    }
}
