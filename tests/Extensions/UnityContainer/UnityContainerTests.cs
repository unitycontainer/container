using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;
using Unity.Abstractions.Tests;
using Unity.Injection;
using Unity.Lifetime;

namespace Extensions.Tests
{
    [TestClass]
    public partial class UnityContainerTests
    {
        private FakeUnityContainer container;
        private InjectionMember[] members;
        private LifetimeManager manager;
        private string name;
        private IUnityContainer unity = null;
        FakeManager FakeManager = new FakeManager();

        [TestInitialize]
        public void InitializeTest() 
        {
            container = new FakeUnityContainer();
            container.Add(typeof(IUnityContainer), container);

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
