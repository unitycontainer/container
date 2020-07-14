using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.BuiltIn;
using Unity.Storage;

namespace Container.Scope
{
    [TestClass]
    public partial class ScopeTests
    {
        protected Unity.Container.Scope Scope;
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
            Scope = Container._scope;
        }

        [TestMethod]
        public void Baseline()
        {
            // Act
            var hash = (uint)typeof(ScopeTests).GetHashCode();
            var zero = typeof(ScopeTests).GetHashCode(0);

            // Validate
            Assert.AreEqual(hash, zero);
            Assert.AreEqual(3, Scope.Contracts);
            Assert.AreEqual(3, Scope.Version);
        }
    }

    public class TestScope : ContainerScope
    {
        public TestScope(ContainerScope scope) 
            : base(scope) { }

        public int ContractMax => _identityMax;
        public int RegistryMax => _registryMax;

        public int ContractPrime => _identityPrime;

        public int ContractCount => _identityCount;

        public object ContractSync => _contractSync;
        public object RegistrySync => _disposables;

        public Metadata[] ContractMeta => _identityMeta;
        public Metadata[] RegistryMeta => _registryMeta;

        public Identity[] ContractData => _identityData;
        public Registry[] RegistryData => _registryData;

        public int GetIndexOf(string name)
        {
            // Same hash to generate collisions
            var hash = (uint)"123".GetHashCode();
            return IndexOf(hash, name, 3);
        }
    }
}
