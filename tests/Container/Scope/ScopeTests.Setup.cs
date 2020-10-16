using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.BuiltIn;
using Unity.Lifetime;
using static Unity.Container.Scope;

namespace Container.Scope
{
    [TestClass]
    public partial class ScopeTests
    {
        protected string Name = "name";
        protected static Type[] TestTypes;
        protected static string[] TestNames;
        protected static LifetimeManager Manager = new ContainerControlledLifetimeManager();
        protected static RegistrationDescriptor[] Registrations;

        protected Unity.Container.Scope Scope;

        [ClassInitialize]
        public static void InitializeClass(TestContext _)
        {
            var DefinedTypes = Assembly.GetAssembly(typeof(int)).DefinedTypes;
            TestNames = Enumerable.Repeat<string>(null, 4)
                .Concat(DefinedTypes.Select(t => t.Name).Take(5))
                .Concat(DefinedTypes.Select(t => t.Name).Distinct().Take(100))
                .ToArray();
            TestTypes = DefinedTypes.Where(t => t != typeof(IServiceProvider))
                                    .Take(2000)
                                    .ToArray();

            var size = 0;
            var position = 0;

            Registrations = TestNames.Select(name =>
            {
                var types = new Type[(++size & 0x7F)];

                Array.Copy(TestTypes, position, types, 0, types.Length);
                position = (position + types.Length) & 0xFF;

                return new RegistrationDescriptor(name, Manager, types);
            }).ToArray();

        }

        [TestInitialize]
        public virtual void InitializeTest() => Scope = new ContainerScope(1);

        [TestMethod]
        public void Baseline()
        {
            Assert.IsNotNull(Scope);
            Assert.AreEqual(0, Scope.Version);
            Assert.AreEqual(0, Scope.Count);
            Assert.AreEqual(0, Scope.ToArray().Length);
            Assert.IsTrue(ReferenceEquals(null, null));

            Assert.AreEqual("public virtual void InitializeTest() =>".GetHashCode(),
                            "public virtual void InitializeTest() =>".GetHashCode());
        }
    }

    public static class ScopeTestExtensions
    {
        public static Entry[] ToArray(this Unity.Container.Scope sequence) 
            => sequence.Memory.Span.ToArray();
    }
}
