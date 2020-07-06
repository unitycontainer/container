using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }

        #endregion


        #region Instance

        public IUnityContainer RegisterFactory(ResolveDelegate<IResolveContext> factory, string? name, IFactoryLifetimeManager manager, params Type[] registerAs)
        {
            throw new NotImplementedException();
        }


        #endregion


        #region Factory

        public IUnityContainer RegisterInstance(object? instance, string? name, IInstanceLifetimeManager manager, params Type[] registerAs)
        {
            throw new NotImplementedException();
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


        #region Registrations

        /// <inheritdoc />
        public bool IsRegistered(Type type, string? name) => throw new NotImplementedException();// _scope.IsExplicitlyRegistered(type, name);

        /// <inheritdoc />
        public IEnumerable<ContainerRegistration> Registrations => _scope.Registrations;

        #endregion


        #region Child Container

        /// <inheritdoc />
        IUnityContainer IUnityContainer.CreateChildContainer(string? name) => CreateChildContainer(name);
        
        #endregion
    }
}
