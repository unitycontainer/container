using System;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer : IUnityContainer
    {
        #region Properties

        /// <inheritdoc />
        IUnityContainer? IUnityContainer.Parent => Parent;



        #endregion


        #region Type

        public IUnityContainer RegisterType(Type type, string? name, ITypeLifetimeManager manager, params Type[] registerAs)
        {
            ReadOnlySpan<RegistrationDescriptor> span = new[] { new RegistrationDescriptor(type, name, manager, registerAs) };
            
            // Add to container 
            var container = manager is SingletonLifetimeManager ? Root : this;
            container._scope.Add(in span);

            // Report registration
            _registering?.Invoke(container, in span);

            return this;
        }

        #endregion


        #region Instance

        public IUnityContainer RegisterFactory(ResolveDelegate<IResolveContext> factory, string? name, IFactoryLifetimeManager manager, params Type[] registerAs)
        {
            ReadOnlySpan<RegistrationDescriptor> span = new[] { new RegistrationDescriptor(factory, name, manager, registerAs) };

            // Add to container 
            var container = manager is SingletonLifetimeManager ? Root : this;
            container._scope.Add(in span);

            // Report registration
            _registering?.Invoke(container, in span);

            return this;
        }


        #endregion


        #region Factory

        public IUnityContainer RegisterInstance(object? instance, string? name, IInstanceLifetimeManager manager, params Type[] registerAs)
        {
            ReadOnlySpan<RegistrationDescriptor> span = new[] { new RegistrationDescriptor(instance, name, manager, registerAs) };

            // Add to container 
            var container = manager is SingletonLifetimeManager ? Root : this;
            container._scope.Add(in span);

            // Report registration
            _registering?.Invoke(container, in span);

            return this;
        }

        #endregion


        #region Resolution

        /// <inheritdoc />
        public object? Resolve(Type type, string? name, params ResolverOverride[] overrides)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public object BuildUp(Type type, object existing, string? name, params ResolverOverride[] overrides)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Child Container

        /// <inheritdoc />
        IUnityContainer IUnityContainer.CreateChildContainer(string? name) => CreateChildContainer(name);
        
        #endregion
    }
}
