using System;
using System.Security;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Resolution;

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

            LifetimeManager = manager;

            ResolveDelegate<BuilderContext> resolver = (ref BuilderContext context) => throw new InvalidOperationException("Instance value no longer available");
            Set(typeof(ResolveDelegate<BuilderContext>), resolver);
        }
    }
}
