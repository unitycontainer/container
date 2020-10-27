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
    public partial class Registrations
    {
        private string Name = "name";
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
        protected static RegistrationDescriptor[] AllRegistrations = TestNames.Select(name =>
        {
            var types = new Type[(++size & 0x7F)];

            Array.Copy(TestTypes, position, types, 0, types.Length);
            position = (position + types.Length) & 0x4FF;

            return new RegistrationDescriptor(Instance, name, Manager, types);
        }).ToArray();

        private UnityContainer Container;


        [TestInitialize]
        public virtual void InitializeTest()
        {
            Container = new UnityContainer();
        }


    }
}
