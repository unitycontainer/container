using System;
using System.Diagnostics;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.Registration
{
    public class FactoryRegistration : ExplicitRegistration
    {
        public FactoryRegistration(UnityContainer owner, Type type, string? name, Func<IUnityContainer, Type, string?, object?> factory, LifetimeManager manager) 
            : base(owner, name, type, manager)
        {
            manager.InUse = true;

            // If Disposable add to container's lifetime
            if (manager is IDisposable managerDisposable)
                owner.Context.Lifetime.Add(managerDisposable);

            LifetimeManager = manager;
            //PipelineDelegate = OnResolve;

            // Factory resolver
            Pipeline = manager switch
            {
                PerResolveLifetimeManager _ => PerResolveLifetime,
                _ => (ResolveDelegate<BuilderContext>)OtherLifetime
            };

            object? PerResolveLifetime(ref BuilderContext context)
            {
                var value = factory(context.Container, context.Type, context.Name);
                context.Set(typeof(LifetimeManager), new InternalPerResolveLifetimeManager(value));
                return value;
            };

            object? OtherLifetime(ref BuilderContext context)
            {
                return factory(context.Container, context.Type, context.Name);
            }
        }
    }
}
