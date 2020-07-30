using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;
using Unity.Abstractions.Tests;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace Extensions.Tests
{
    [TestClass]
    public partial class UnityContainerTests
    {
        FakeUnityContainer container;
        InjectionMember[] members;
        LifetimeManager manager;
        IResolveContext context;
        string name;
        IUnityContainer unity = null;
        FakeManager FakeManager = new FakeManager();

        [TestInitialize]
        public void InitializeTest() 
        {
            container = new FakeUnityContainer();
            container.Add(typeof(IUnityContainer), container);

            context = new DictionaryContext() { { typeof(IUnityContainer), container } };

            members = new InjectionMember[] { };
            manager = new ContainerControlledLifetimeManager();
            name = "name";
        }
    }


    #region Test Data

    public class FakeManager : IFactoryLifetimeManager,
                               ITypeLifetimeManager, 
                               IInstanceLifetimeManager
    {
        public LifetimeManager Clone() => throw new NotImplementedException();
    }

    #endregion
}
