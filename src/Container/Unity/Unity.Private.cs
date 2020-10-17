using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Lifetime;

namespace Unity
{
    public partial class UnityContainer
    {
        private void DisposeManager(RegistrationManager? manager)
        { 
        
        }

        #region Default Lifetime Manager

        private ITypeLifetimeManager DefaultTypeLifetimeManager(Type type)
        {
            PartCreationPolicyAttribute? attribute;

            if (null == (attribute = (PartCreationPolicyAttribute?)type.GetCustomAttribute(typeof(PartCreationPolicyAttribute))))
                return new TransientLifetimeManager();

            return CreationPolicy.NonShared == attribute.CreationPolicy
                ? (ITypeLifetimeManager)new TransientLifetimeManager()
                : new ContainerControlledLifetimeManager();
        }

        private IInstanceLifetimeManager DefaultInstanceLifetimeManager(Type type)
        {
            PartCreationPolicyAttribute? attribute;

            if (null == (attribute = (PartCreationPolicyAttribute?)type.GetCustomAttribute(typeof(PartCreationPolicyAttribute))))
                return new ContainerControlledLifetimeManager();

            return CreationPolicy.NonShared == attribute.CreationPolicy
                ? (IInstanceLifetimeManager)new TransientLifetimeManager()
                : new ContainerControlledLifetimeManager();
        }

        private IFactoryLifetimeManager DefaultFactoryLifetimeManager(Type type)
        {
            PartCreationPolicyAttribute? attribute;

            if (null == (attribute = (PartCreationPolicyAttribute?)type.GetCustomAttribute(typeof(PartCreationPolicyAttribute))))
                return new TransientLifetimeManager();

            return CreationPolicy.NonShared == attribute.CreationPolicy
                ? (IFactoryLifetimeManager)new TransientLifetimeManager()
                : new ContainerControlledLifetimeManager();
        }

        #endregion
    }
}
