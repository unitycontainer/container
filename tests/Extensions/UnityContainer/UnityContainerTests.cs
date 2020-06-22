using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        [TestInitialize]
        public void InitializeTest() 
        {
            container = new FakeUnityContainer() { { typeof(IUnityContainer), container } };

            members = new InjectionMember[] { };
            manager = new ContainerControlledLifetimeManager();
            name = "name";
        }
    }
}
