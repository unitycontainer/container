using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.BuiltIn;
using Unity.Container;

namespace Container.Scope
{
    [TestClass]
    public partial class ContainerScopeTests
    {
        ContainerScope Scope;
        static RegistrationDescriptor[] Registrations;

        [ClassInitialize]
        public static void InitializeData(TestContext _)
        {
            var size = 0;
            var position = 0;
            var stringCount = 100;
            var DefinedTypes = Assembly.GetAssembly(typeof(int)).DefinedTypes;
            var manager = new ContainerLifetimeManager(new object());
            var names = Enumerable.Repeat<string>(null, 4)
                .Concat(DefinedTypes.Take(5).Select(t => t.Name))
                .Concat(DefinedTypes.Take(stringCount).Select(t => t.Name))
                .ToArray();
            Type[] TestTypes = DefinedTypes.Where(t => t != typeof(IServiceProvider)).ToArray();

            Registrations = names.Select(name =>
            {
                var types = new Type[(++size & 0x7F)];

                Array.Copy(TestTypes, position, types, 0, types.Length);
                position = (position + types.Length) & 0x7FF;

                return new RegistrationDescriptor(name, manager, types);
            }).ToArray();

        }

        [TestInitialize]
        public void InitializeTest() => Scope = new ContainerScope();

        [TestMethod]
        public void Baseline()
        {
            Assert.IsNotNull(Scope);
            Assert.AreEqual(0, Scope.ToArray().Length);
        }
    }
}
