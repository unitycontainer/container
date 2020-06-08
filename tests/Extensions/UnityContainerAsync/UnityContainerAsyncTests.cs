using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace Extensions.Tests
{
    [TestClass]
    public partial class UnityContainerAsyncTests
    {
        private FakeIUCA container;
        private InjectionMember[] members;
        private LifetimeManager manager;
        private string name;
        private IUnityContainerAsync unity = null;

        [TestInitialize]
        public void InitializeTest() 
        {
            container = new FakeIUCA();
            members = new InjectionMember[] { };
            manager = new ContainerControlledLifetimeManager();
            name = "name";
        }

        [TestMethod]
        public void BaselineTest()
        {
            IUnityContainerAsync container = new FakeIUCA();
            
            Assert.IsNotNull(container);
        }
    }

    #region Test Data

    public class FakeIUCA : IUnityContainerAsync
    {
        public Type Type { get; private set; }
        public string Name { get; private set; }
        public Type MappedTo { get; private set; }
        public LifetimeManager LifetimeManager { get; private set; }
        public InjectionMember[] InjectionMembers { get; private set; }
        public ResolverOverride[] ResolverOverrides { get; private set; }
        public object Data { get; set; }

        public IEnumerable<IContainerRegistration> Registrations => throw new NotImplementedException();

        public IUnityContainerAsync Parent => throw new NotImplementedException();

        public IUnityContainerAsync CreateChildContainer()
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

        public Task RegisterFactory(IEnumerable<Type> interfaces, string name, Func<IUnityContainer, Type, string, object> factory, IFactoryLifetimeManager lifetimeManager)
        {
            throw new NotImplementedException();
        }

        public Task RegisterInstance(IEnumerable<Type> interfaces, string name, object instance, IInstanceLifetimeManager lifetimeManager)
        {
            throw new NotImplementedException();
        }

        public Task RegisterType(IEnumerable<Type> interfaces, Type type, string name, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IEnumerable<object>> Resolve(Type type, Regex regex, params ResolverOverride[] overrides)
        {
            throw new NotImplementedException();
        }

        public ValueTask<object> ResolveAsync(Type type, string name, params ResolverOverride[] overrides)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}
