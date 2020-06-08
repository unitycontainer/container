using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace Extensions.Tests
{
    [TestClass]
    public partial class UnityContainerTests
    {
        private FakeIUC container;
        private InjectionMember[] members;
        private LifetimeManager manager;
        private string name;
        private IUnityContainer unity = null;

        [TestInitialize]
        public void InitializeTest() 
        {
            container = new FakeIUC();
            members = new InjectionMember[] { };
            manager = new ContainerControlledLifetimeManager();
            name = "name";
        }
    }

    #region Test Data

    public class FakeIUC : IUnityContainer
    {
        public Type Type { get; private set; }
        public string Name { get; private set; }
        public Type MappedTo { get; private set; }
        public LifetimeManager LifetimeManager { get; private set; }
        public InjectionMember[] InjectionMembers { get; private set; }
        public ResolverOverride[] ResolverOverrides { get; private set; }
        public object Data { get; set; }

        public IEnumerable<IContainerRegistration> Registrations => throw new NotImplementedException();

        public IUnityContainer Parent => throw new NotImplementedException();

        public object BuildUp(Type type, object existing, string name, params ResolverOverride[] overrides)
        {
            Type = type;
            Name = name;
            Data = existing;

            return Data;
        }

        public IUnityContainer CreateChildContainer()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool IsRegistered(Type type, string name)
        {
            Type = type;
            Name = name;

            return true;
        }

        public IUnityContainer RegisterFactory(Type type, string name, Func<IUnityContainer, Type, string, object> factory, IFactoryLifetimeManager lifetimeManager)
        {
            Type = type;
            Name = name;
            Data = factory;
            LifetimeManager = (LifetimeManager)lifetimeManager;

            return this;
        }

        public IUnityContainer RegisterInstance(Type type, string name, object instance, IInstanceLifetimeManager lifetimeManager)
        {
            Type = type;
            Name = name;
            Data = instance;
            LifetimeManager = (LifetimeManager)lifetimeManager;

            return this;
        }

        public IUnityContainer RegisterType(Type registeredType, Type mappedToType, string name, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            Type = registeredType;
            MappedTo = mappedToType;
            Name = name;
            LifetimeManager = (LifetimeManager)lifetimeManager;
            InjectionMembers = injectionMembers;

            return this;
        }

        public object Resolve(Type type, string name, params ResolverOverride[] overrides)
        {
            Type = type;
            Name = name;
            ResolverOverrides = overrides;
            
            return Data;
        }
    }

    #endregion
}
