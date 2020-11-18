using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using Unity.Lifetime;

namespace Unity.Benchmarks
{
    [TestClass]
    public class ContainerAPI
    {
        #region Scaffolding

        protected IUnityContainer Container;
        protected static string Name = "name";

        protected static IInstanceLifetimeManager Manager = new ContainerControlledLifetimeManager();
        private static IEnumerable<TypeInfo> DefinedTypes = Assembly.GetAssembly(typeof(int)).DefinedTypes.Take(1000);
        protected static string[] TestNames = Enumerable.Repeat<string>(null, 4)
                                                        .Concat(DefinedTypes.Take(5).Select(t => t.Name))
                                                        .Concat(DefinedTypes.Select(t => t.Name).Distinct().Take(100))
                                                        .ToArray();
        protected static Type[] TestTypes = DefinedTypes.Where(t => t != typeof(IServiceProvider))
                                                        .ToArray();
        static int size = 0;
        static int position = 0;
        protected static object Instance = new object();
        protected static RegistrationDescriptor[] RegistrationsData = TestNames.Select(name =>
        {
            var types = new Type[(++size & 0x7F)];

            Array.Copy(TestTypes, position, types, 0, types.Length);
            position = (position + types.Length) & 0x4FF;

            return new RegistrationDescriptor(Instance, name, Manager, types);
        }).ToArray();


        [TestInitialize]
        public void GlobalSetup()
        {
            Container = new UnityContainer();
            Container.Register(RegistrationsData);
        }

        #endregion


        [TestMethod]
        public void Registrations()
        {
            // Act
            Assert.IsNotNull(Container.Registrations);
        }

        [TestMethod]
        public void RegistrationsToArray()
        {
            var registrations = Container.Registrations.ToArray();

            Assert.IsNotNull(registrations);
        }


        [TestMethod]
        public void RegistrationsToArrayTwice()
        {
            var registrations1 = Container.Registrations.ToArray();
            var registrations2 = Container.Registrations.ToArray();

            Assert.AreEqual(registrations1.Length, registrations2.Length);
        }


        [Import]
        public int Property { get; set; }

        [TestMethod]
        public void GetCustomAttributesAll()
        {
            var property = TypeDescriptor.GetProperties(typeof(ContainerAPI))[nameof(Property)];
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(typeof(ContainerAPI))[nameof(Property)];

            var attributes = descriptor.Attributes[typeof(ImportAttribute)];

            Assert.IsNotNull(property);
        }

        private PropertyInfo property = typeof(ContainerAPI).GetTypeInfo().GetProperty(nameof(Property));

        [TestMethod]
        public void GetCustomAttribute()
        {

            //var data = typeof(ImportAttribute).GetEnumUnderlyingType();
            var module = property.DeclaringType.Module;
            //var token = property.MetadataToken;

            var car = CustomAttributeData.GetCustomAttributes(module);

            Assert.IsNotNull(property.GetCustomAttribute(typeof(ImportAttribute), true));
        }

        [TestMethod]
        public void GetCustomAttributesType()
        {
            Assert.IsNotNull(property.GetCustomAttributes(typeof(ImportAttribute), true));
        }

    }
}
