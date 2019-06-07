using System;
using System.Diagnostics;
using System.Security;
using Unity.Builder;
using Unity.Lifetime;

namespace Unity.Registration
{
    [SecuritySafeCritical]
    public class InstanceRegistration : ExplicitRegistration
    {
        public InstanceRegistration(UnityContainer owner, Type? type, string? name, object? instance, LifetimeManager manager)
            : base(owner, name, instance?.GetType() ?? type)
        {
            // If Disposable register with the container
            if (manager is IDisposable disposableManager) owner.Context.Lifetime.Add(disposableManager);

            // Setup Manager
            manager.InUse = true;
            manager.SetValue(instance, owner.Context.Lifetime);
            Pipeline = (ref BuilderContext context) => throw new InvalidOperationException("Instance value no longer available");

            LifetimeManager = manager;
        }

        #region Implementation

        private object ExternalLifetime(ref BuilderContext context)
        {
            Debug.Assert(null != LifetimeManager);
            var value = LifetimeManager.GetValue(context.ContainerContext.Lifetime);

            // Externally controlled lifetime can go out of scope, check if still valid
            if (LifetimeManager.NoValue == value)
                throw new ObjectDisposedException(Type?.Name, "Externally controlled object has been already disposed.");

            return value;
        }

        private object OtherLifetime(ref BuilderContext context)
        {
            Debug.Assert(null != LifetimeManager);
            return LifetimeManager.GetValue(context.ContainerContext.Lifetime);
        }

        #endregion
    }
}
