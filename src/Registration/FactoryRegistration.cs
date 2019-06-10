using System;
using Unity;
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

            ResolveDelegate<PipelineContext> resolver = (ref PipelineContext context) => factory(context.Container, context.Type, context.Name);
            Set(typeof(ResolveDelegate<PipelineContext>), resolver);
        }
    }
}
