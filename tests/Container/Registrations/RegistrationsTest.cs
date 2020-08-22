using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.Lifetime;

namespace Container.Registrations
{
    [TestClass]
    public class RegistrationsTests
    {
        protected static IInstanceLifetimeManager Manager = new ContainerControlledLifetimeManager();
        private static IEnumerable<TypeInfo> DefinedTypes = Assembly.GetAssembly(typeof(int)).DefinedTypes;
        protected static string[] TestNames = Enumerable.Repeat<string>(null, 4)
                                                        .Concat(DefinedTypes.Take(5).Select(t => t.Name))
                                                        .Concat(DefinedTypes.Select(t => t.Name).Distinct().Take(100))
                                                        .ToArray();
        protected static Type[] TestTypes = DefinedTypes.Where(t => t != typeof(IServiceProvider))
                                                        .ToArray();
        static int size = 0;
        static int position = 0;
        protected static object Instance = new object();
        protected static RegistrationDescriptor[] Registrations = TestNames.Select(name =>
        {
            var types = new Type[(++size & 0x7F)];

            Array.Copy(TestTypes, position, types, 0, types.Length);
            position = (position + types.Length) & 0x7FF;

            return new RegistrationDescriptor(Instance, name, Manager, types);
        }).ToArray();

        [TestMethod]
        public void RegisterDynamic()
        {
            var container = new UnityContainer();
            container.Register(Registrations);

            var array = container.Registrations.ToArray();
            Assert.AreEqual(5998, array.Length);
        }

        [TestMethod]
        public void RegisterPreallocated()
        {
            var container = new UnityContainer(10000);
            container.Register(Registrations);

            var array = container.Registrations.ToArray();
            Assert.AreEqual(5998, array.Length);
        }

    }
}
