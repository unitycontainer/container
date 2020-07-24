using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.BuiltIn;
using Unity.Container;
using Unity.Lifetime;

namespace Container.Scope
{
    [TestClass]
    public partial class ScopeTests
    {
        protected string Name = "name";
        protected static Type[] TestTypes;
        protected static string[] TestNames;
        protected static LifetimeManager Manager = new ContainerLifetimeManager("Test Manager");
        protected static RegistrationDescriptor[] Registrations;

        protected Unity.Container.Scope Scope;

        [ClassInitialize]
        public static void InitializeClass(TestContext _)
        {
            var DefinedTypes = Assembly.GetAssembly(typeof(int)).DefinedTypes;
            TestNames = Enumerable.Repeat<string>(null, 4)
                .Concat(DefinedTypes.Take(5).Select(t => t.Name))
                .Concat(DefinedTypes.Take(100).Select(t => t.Name))
                .ToArray();
            TestTypes = DefinedTypes.Where(t => t != typeof(IServiceProvider))
                                    .ToArray();

            var size = 0;
            var position = 0;

            Registrations = TestNames.Select(name =>
            {
                var types = new Type[(++size & 0x7F)];

                Array.Copy(TestTypes, position, types, 0, types.Length);
                position = (position + types.Length) & 0x7FF;

                return new RegistrationDescriptor(name, Manager, types);
            }).ToArray();

        }

        [TestInitialize]
        public virtual void InitializeTest() => Scope = new ContainerScope();

        [TestMethod]
        public void Baseline()
        {
            Assert.IsNotNull(Scope);
            Assert.AreEqual(0, Scope.Names);
            Assert.AreEqual(0, Scope.Version);
            Assert.AreEqual(0, Scope.Contracts);
            Assert.AreEqual(0, Scope.ToArray().Length);
            Assert.IsTrue(ReferenceEquals(null, null));
        }
    }
}
