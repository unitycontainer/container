using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.Container;
using Unity.Lifetime;

namespace Container.Interfaces
{
    [TestClass]
    public partial class UnityInterfacesTests
    {
        static int size = 0;
        static int position = 0;
        static IEnumerable<TypeInfo> DefinedTypes = Assembly.GetAssembly(typeof(int)).DefinedTypes;

        // Registration snooping values
        protected int count;
        protected int called;
        protected object sender;

        protected string Name = "name";
        protected static Type[] TestTypes = DefinedTypes.Where(t => t != typeof(IServiceProvider))
                                                        .ToArray();
        protected static string[] TestNames = Enumerable.Repeat<string>(null, 4)
                .Concat(DefinedTypes.Take(5).Select(t => t.Name))
                .Concat(DefinedTypes.Take(100).Select(t => t.Name))
                .ToArray();
        protected static LifetimeManager Manager = new ContainerLifetimeManager("Test Manager");
        protected UnityContainer Container;
        protected static RegistrationDescriptor[] Registrations = 
            TestNames.Select(name =>
            {
                var types = new Type[(++size & 0x7F)];

                Array.Copy(TestTypes, position, types, 0, types.Length);
                position = (position + types.Length) & 0x7FF;

                return new RegistrationDescriptor(name, Manager, types);
            }).ToArray();


        [TestInitialize]
        public virtual void InitializeTest()
        {
            count = 0;
            called = 0;
            sender = null;

            Container = new UnityContainer();
        }
    }
}
