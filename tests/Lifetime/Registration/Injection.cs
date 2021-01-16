using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Lifetime
{
    public partial class Managers
    {
        [TestMethod("Baseline values"), TestProperty(BASE_TYPE, INJECTION)]
        public void Base_Baseline()
        {
            // Arrange
            var manager = new TransientLifetimeManager();

            // Validate
            Assert.IsNull(manager.Constructor);
            Assert.IsNull(manager.Fields);
            Assert.IsNull(manager.Properties);
            Assert.IsNull(manager.Methods);
        }

        [TestMethod(".ctor(arguments)"), TestProperty(BASE_TYPE, INJECTION)]
        public void Base_Ctor()
        {
            // Arrange
            var sequence = new InjectionMember[]
            {
                new InjectionConstructor(),
                new InjectionMethod(string.Empty)
            };

            // Act
            var manager = new TransientLifetimeManager(sequence[0], sequence[1]);

            // Validate
            Assert.IsNotNull(manager.Constructor);
            Assert.IsNull(manager.Fields);
            Assert.IsNull(manager.Properties);
            Assert.IsNotNull(manager.Methods);
        }

        [TestMethod("Using initializer"), TestProperty(BASE_TYPE, INJECTION)]
        public void Base_Initializer()
        {
            // Arrange
            var sequence = new InjectionMember[]
            {
                new InjectionConstructor(),
                new InjectionMethod(string.Empty)
            };

            // Act
            var manager = new TransientLifetimeManager { sequence[0], sequence[1] };

            // Validate
            Assert.IsNotNull(manager.Constructor);
            Assert.IsNull(manager.Fields);
            Assert.IsNull(manager.Properties);
            Assert.IsNotNull(manager.Methods);
        }

        [TestMethod(".ctor(arguments) and Initializer"), TestProperty(BASE_TYPE, INJECTION)]
        public void Base_CtorInitializer()
        {
            // Arrange
            var sequence = new InjectionMember[]
            {
                new InjectionConstructor(),
                new InjectionMethod(string.Empty),
                new InjectionProperty(string.Empty),
                new InjectionField(string.Empty)
            };

            // Act
            var manager = new TransientLifetimeManager(sequence[0], sequence[1])
            {
                sequence[2], sequence[3]
            };

            // Validate
            Assert.IsNotNull(manager.Constructor);
            Assert.IsNotNull(manager.Fields);
            Assert.IsNotNull(manager.Properties);
            Assert.IsNotNull(manager.Methods);
        }

        [TestMethod("Add(...)"), TestProperty(BASE_TYPE, INJECTION)]
        public void Base_AddEmpty()
        {
            // Arrange
            var sequence = new InjectionMember[] { new InjectionConstructor() };
            var manager = new TransientLifetimeManager();

            // Act
            manager.Add(sequence[0]);

            // Validate
            Assert.IsNotNull(manager.Constructor);
            Assert.IsNull(manager.Fields);
            Assert.IsNull(manager.Properties);
            Assert.IsNull(manager.Methods);
        }


        [TestMethod(".ctor(arguments) and Add()"), TestProperty(BASE_TYPE, INJECTION)]
        public void Base_CtorAdd()
        {
            // Arrange
            var sequence = new InjectionMember[]
            {
                new InjectionConstructor(),
                new InjectionMethod(string.Empty),
                new InjectionProperty(string.Empty),
                new InjectionField(string.Empty)
            };
            var manager = new TransientLifetimeManager(sequence[0], sequence[1]);

            // Act
            manager.Add(sequence[2]);
            manager.Add(sequence[3]);

            // Validate
            Assert.IsNotNull(manager.Constructor);
            Assert.IsNotNull(manager.Fields);
            Assert.IsNotNull(manager.Properties);
            Assert.IsNotNull(manager.Methods);
        }


        [TestMethod(".ctor(arguments), Initializer and Add()"), TestProperty(BASE_TYPE, INJECTION)]
        public void Base_CtorInitializerAdd()
        {
            // Arrange
            var sequence = new InjectionMember[]
            {
                new InjectionConstructor(),
                new InjectionMethod(string.Empty),
                new InjectionProperty(string.Empty),
                new InjectionField(string.Empty)
            };
            var manager = new TransientLifetimeManager(sequence[0])
            {
                sequence[1]
            };

            // Act
            manager.Add(sequence[2]);
            manager.Add(sequence[3]);

            // Validate
            Assert.IsNotNull(manager.Constructor);
            Assert.IsNotNull(manager.Fields);
            Assert.IsNotNull(manager.Properties);
            Assert.IsNotNull(manager.Methods);
        }

        [TestMethod("Add(range)"), TestProperty(BASE_TYPE, INJECTION)]
        public void Base_AddRangeEmpty()
        {
            // Arrange
            var sequence = new InjectionMember[]
            {
                new InjectionConstructor(),
                new InjectionMethod(string.Empty),
                new InjectionProperty(string.Empty),
                new InjectionField(string.Empty)
            };
            var manager = new TransientLifetimeManager();

            // Act
            manager.Add(sequence);

            // Validate
            Assert.IsNotNull(manager.Constructor);
            Assert.IsNotNull(manager.Fields);
            Assert.IsNotNull(manager.Properties);
            Assert.IsNotNull(manager.Methods);
        }


        [TestMethod(".ctor(arguments) and Add(range)"), TestProperty(BASE_TYPE, INJECTION)]
        public void Base_CtorAddRange()
        {
            // Arrange
            var sequence = new InjectionMember[]
            {
                new InjectionConstructor(),
                new InjectionMethod(string.Empty),
                new InjectionProperty(string.Empty),
                new InjectionField(string.Empty)
            };
            var range = new[] { sequence[2], sequence[3] };
            var manager = new TransientLifetimeManager(sequence[0], sequence[1]);

            // Act
            manager.Add(range);

            // Validate
            Assert.IsNotNull(manager.Constructor);
            Assert.IsNotNull(manager.Fields);
            Assert.IsNotNull(manager.Properties);
            Assert.IsNotNull(manager.Methods);
        }


        [TestMethod(".ctor(arguments), Initializer and Add(range)"), TestProperty(BASE_TYPE, INJECTION)]
        public void Base_CtorInitializerAddRange()
        {
            // Arrange
            var sequence = new InjectionMember[]
            {
                new InjectionConstructor(),
                new InjectionMethod(string.Empty),
                new InjectionProperty(string.Empty),
                new InjectionField(string.Empty)
            };
            var range = new[] { sequence[2], sequence[3] };
            var manager = new TransientLifetimeManager(sequence[0])
            {
                sequence[1]
            };

            // Act
            manager.Add(range);

            // Validate
            Assert.IsNotNull(manager.Constructor);
            Assert.IsNotNull(manager.Fields);
            Assert.IsNotNull(manager.Properties);
            Assert.IsNotNull(manager.Methods);
        }


        [DataTestMethod, TestProperty(BASE_TYPE, INJECTION)]
        [DynamicData(nameof(GetManagers), DynamicDataSourceType.Method)]
        public void Base_AddInjectionMember(RegistrationManager manager)
        {
            // Validate
            Assert.IsNull(manager.Data);
            Assert.AreEqual(RegistrationCategory.Uninitialized, manager.Category);

            // Can assign
            manager.Data = this;
            manager.Category = RegistrationCategory.Instance;
            Assert.AreSame(this, manager.Data);
            Assert.AreEqual(RegistrationCategory.Instance, manager.Category);
        }

        public static IEnumerable<object[]> GetManagers()
        {
            yield return new object[] { new ContainerControlledLifetimeManager() };
            yield return new object[] { new ContainerControlledTransientManager() };
            yield return new object[] { new ExternallyControlledLifetimeManager() };
            yield return new object[] { new HierarchicalLifetimeManager() };
            yield return new object[] { new PerResolveLifetimeManager() };
            yield return new object[] { new PerThreadLifetimeManager() };
            yield return new object[] { new TransientLifetimeManager() };
        }

    }
}
