using System;
using System.Diagnostics;
using System.Security;
using System.Threading.Tasks;
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
            if (manager is IDisposable disposableManager)
                owner.Context.Lifetime.Add(disposableManager);

            // Set Value
            manager.InUse = true;
            manager.SetValue(instance, owner.Context.Lifetime);

            // Set Members
            LifetimeManager = manager;
            PipelineDelegate = OnResolve;
            Pipeline = manager switch
            {
                ExternallyControlledLifetimeManager _ => ExternalLifetime,
                _ => (ResolveDelegate<BuilderContext>)   OtherLifetime
            };
        }

        #region Implementation

        [SecuritySafeCritical]
        private ValueTask<object?> OnResolve(ref BuilderContext context)
        {
            Debug.Assert(null != Pipeline);
            return new ValueTask<object?>(Pipeline(ref context));
        }

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
