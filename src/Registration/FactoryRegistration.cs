using System;
using Unity.Builder;
using Unity.Lifetime;

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

            // Factory resolver
            if (manager is PerResolveLifetimeManager)
            {
                Pipeline = (ref BuilderContext context) =>
                {
                    var value = factory(context.Container, context.Type, context.Name);
                    context.Set(typeof(LifetimeManager), new RuntimePerResolveLifetimeManager(value));
                    return value;
                };
            }
            else
            {
                Pipeline = (ref BuilderContext context) => factory(context.Container, context.Type, context.Name);
            }
        }
    }
}
