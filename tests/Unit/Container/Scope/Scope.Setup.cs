using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.Container;
using Unity.Storage;

namespace Container.Scope
{
    [TestClass]
    public partial class ScopeTests
    {
        protected TestScope      Scope;
        protected UnityContainer Container;
        protected string Name = "0123456789";
        protected const int StartPosition = 4;
        protected const int SizeTypes = 200;
        protected const int SizeNames = 100;
        protected static Type[] TestTypes = Assembly.GetAssembly(typeof(int))
                                                    .DefinedTypes
                                                    .Where(t => t != typeof(IServiceProvider))
                                                    .Take(SizeTypes)
                                                    .ToArray();
        string[] TestNames = TestTypes.Take(SizeNames)
                                      .Select(t => t.Name)
                                      .ToArray();

        protected virtual UnityContainer GetContainer() => new UnityContainer();

        [TestInitialize]
        public virtual void InitializeTest()
        {
            Container = GetContainer();
            Scope = new TestScope(Container._scope);
        }

        [TestMethod]
        public void Baseline()
        {

            Assert.IsInstanceOfType(Scope, typeof(TestScope));

            Assert.AreEqual(StartPosition - 1, Scope.RegistryCount);

            Assert.AreSame(Container, Scope.Container);
            Assert.AreEqual(typeof(IUnityContainer),      Scope.RegistryData[1].RegisteredType);
            Assert.AreEqual(typeof(IUnityContainerAsync), Scope.RegistryData[2].RegisteredType);
            Assert.AreEqual(typeof(IServiceProvider),     Scope.RegistryData[3].RegisteredType);
        }
    }

    public class TestScope : ContainerScope
    {
        public TestScope(ContainerScope scope) 
            : base(scope) { }

        public int IdentityMax => _identityMax;
        public int RegistryMax => _registryMax;

        public int IdentityPrime => _identityPrime;

        public int IdentityCount => _identityCount;
        public int RegistryCount => _registryCount;

        public object IdentitySync => _identitySync;
        public object RegistrySync => _registrySync;

        public Metadata[] IdentityMeta => _identityMeta;
        public Metadata[] RegistryMeta => _registryMeta;

        public RegistrationContract[] IdentityData => _identityData;
        public ContainerRegistration[] RegistryData => _registryData;
    }
}
