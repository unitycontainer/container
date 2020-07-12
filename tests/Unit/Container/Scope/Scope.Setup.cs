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
            Scope = new TestScope((ContainerScope)Container._scope);
        }

        [TestMethod]
        public void Baseline()
        {
            // Act
            var hash = (uint)typeof(ScopeTests).GetHashCode();
            var zero = typeof(ScopeTests).GetHashCode(0);

            // Validate
            Assert.AreEqual(hash, zero);
            Assert.AreEqual(0, Scope.Count);
            Assert.AreEqual(0, Scope.Version);
            Assert.IsInstanceOfType(Scope, typeof(TestScope));
            Assert.AreEqual(StartPosition - 1, Scope.RegistryCount);
            Assert.AreSame(Container, Scope.Container);
            Assert.AreEqual(typeof(IUnityContainer),      Scope.RegistryData[1].Contract.Type);
            Assert.AreEqual(typeof(IUnityContainerAsync), Scope.RegistryData[2].Contract.Type);
            Assert.AreEqual(typeof(IServiceProvider),     Scope.RegistryData[3].Contract.Type);
        }
    }

    public class TestScope : ContainerScope
    {
        public TestScope(ContainerScope scope) 
            : base(scope) { }

        public int ContractMax => _contractMax;
        public int RegistryMax => _registryMax;

        public int ContractPrime => _contractPrime;

        public int ContractCount => _contracts;
        public int RegistryCount => _registrations;

        public object ContractSync => _manager;
        public object RegistrySync => _lifetimes;

        public Metadata[] ContractMeta => _contractMeta;
        public Metadata[] RegistryMeta => _registryMeta;

        public Identity[] ContractData => _contractData;
        public Registry[] RegistryData => _registryData;

        public int GetIndexOf(string name)
        {
            // Same hash to generate collisions
            var hash = (uint)"123".GetHashCode();
            return IndexOf(hash, name, 3);
        }
    }
}
