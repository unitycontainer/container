using System;
using Unity.Lifetime;

namespace Unity.Container.Registration
{
    public class InstanceRegistration : ContainerRegistration
    {
        #region Constructors

        public InstanceRegistration(Type mapType, string name, object instance, LifetimeManager lifetime)
            : base(mapType, name)
        {
            MappedToType = instance.GetType();
            LifetimeManager = lifetime;
            LifetimeManager.SetValue(instance);
        }

        #endregion
    }
}
