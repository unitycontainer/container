using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
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
        }
    }

    public class TestScope : ContainerScope
    {
        public TestScope(ContainerScope scope) 
            : base(scope) { }

        public int ContractMax => _namesMax;
        public int RegistryMax => _registryMax;

        public int ContractPrime => _namesPrime;

        public int ContractCount => _namesCount;

        public object ContractSync => _namesSync;
        public object RegistrySync => _disposables;

        public Metadata[] ContractMeta => _namesMeta;
        public Metadata[] RegistryMeta => _registryMeta;

        public NameInfo[] ContractData => _namesData;
        public Registration[] RegistryData => _registryData;

        // TODO: Replace
        public int GetIndexOf(string name)
        {
            var length = _namesCount;
            uint hash = (uint)name?.GetHashCode();

            // Check if already registered
            var bucket = hash % _namesMeta.Length;
            var position = _namesMeta[bucket].Position;
            while (position > 0)
            {
                if (_namesData[position].Name == name) return position;
                position = _namesMeta[position].Next;
            }

            lock (_namesSync)
            {
                // Check again if length changed during wait for lock
                if (length != _namesCount)
                {
                    bucket = hash % _namesMeta.Length;
                    position = _namesMeta[bucket].Position;
                    while (position > 0)
                    {
                        if (_namesData[position].Name == name) return position;
                        position = _namesMeta[position].Next;
                    }
                }

                // Expand if required
                if (_namesCount >= _namesMax)
                {
                    var size = Prime.Numbers[++_namesPrime];
                    _namesMax = (int)(size * LoadFactor);

                    Array.Resize(ref _namesData, size);
                    _namesMeta = new Metadata[size];

                    // Rebuild buckets
                    for (var current = START_INDEX; current <= _namesCount; current++)
                    {
                        bucket = _namesData[current].Hash % size;
                        _namesMeta[current].Next = _namesMeta[bucket].Position;
                        _namesMeta[bucket].Position = current;
                    }

                    bucket = hash % _namesMeta.Length;
                }

                _namesData[++_namesCount] = new NameInfo(hash, name, 1);
                _namesMeta[_namesCount].Next = _namesMeta[bucket].Position;
                _namesMeta[bucket].Position = _namesCount;

                return _namesCount;
            }
        }

    }
}
