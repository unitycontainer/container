using System;
using System.Collections.Generic;
using System.Text;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Policy;

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
    internal class ContainerLifetimeManager : LifetimeManager, IResolverPolicy
    {
        private object _value;

        public override object GetValue()
        {
            return _value;
        }

        public override void SetValue(object newValue)
        {
            _value = newValue;
        }

        public override void RemoveValue()
        {
        }

        public object Resolve(IBuilderContext _)
        {
            return _value;
        }
    }
}
