using System;
using System.Collections.Generic;
using System.Text;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.Scope
{
    public partial class InjectionScope : IUnityContainer
    {
        public IEnumerable<IContainerRegistration> Registrations => throw new NotImplementedException();

        public IUnityContainer? Parent => throw new NotImplementedException();

        public object BuildUp(Type type, object existing, string? name, params ResolverOverride[] overrides)
        {
            throw new NotImplementedException();
        }

        public IUnityContainer CreateChildContainer()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool IsRegistered(Type type, string? name)
        {
            throw new NotImplementedException();
        }

        public IUnityContainer RegisterFactory(Type type, string? name, Func<IUnityContainer, Type, string, object?> factory, IFactoryLifetimeManager? lifetimeManager, params InjectionMember[] injectionMembers)
        {
            throw new NotImplementedException();
        }

        public IUnityContainer RegisterInstance(Type? type, string? name, object? instance, IInstanceLifetimeManager? lifetimeManager, params InjectionMember[] injectionMembers)
        {
            throw new NotImplementedException();
        }

        public IUnityContainer RegisterType(Type? registeredType, Type? mappedToType, string? name, ITypeLifetimeManager? lifetimeManager, params InjectionMember[] injectionMembers)
        {
            throw new NotImplementedException();
        }

        public object? Resolve(Type type, string? name, params ResolverOverride[] overrides)
        {
            throw new NotImplementedException();
        }
    }
}
