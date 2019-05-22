using System;
using System.Collections.Generic;
using System.Text;
using Unity.Lifetime;
using Unity.Policy;

namespace Unity.Registration
{
    public static class RegistrationExtensions
    {
        public static bool IsSingletonLifetimeManagerRegistration(this IPolicySet policySet)
        {
            return (policySet as ContainerRegistration)?.LifetimeManager is SingletonLifetimeManager;
        }
    }
}
