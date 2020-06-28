using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer : IUnityContainerAsync
    {
        IUnityContainerAsync? IUnityContainerAsync.Parent => _parent;

        public Task RegisterFactory(IEnumerable<Type>? interfaces, string? name, Func<IUnityContainer, Type, string?, object?> factory, IFactoryLifetimeManager? lifetimeManager, params InjectionMember[] injectionMembers)
        {
            throw new NotImplementedException();
        }

        public Task RegisterInstance(IEnumerable<Type>? interfaces, string? name, object? instance, IInstanceLifetimeManager? lifetimeManager)
        {
            throw new NotImplementedException();
        }

        public Task RegisterType(IEnumerable<Type>? interfaces, Type type, string? name, ITypeLifetimeManager? lifetimeManager, params InjectionMember[] injectionMembers)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IEnumerable<object>> Resolve(Type type, Regex regex, params ResolverOverride[] overrides)
        {
            throw new NotImplementedException();
        }

        public ValueTask<object?> ResolveAsync(Type type, string? name, params ResolverOverride[] overrides)
        {
            throw new NotImplementedException();
        }


        IUnityContainerAsync IUnityContainerAsync.CreateChildContainer(string? name) => CreateChildContainer(name);
    }
}
