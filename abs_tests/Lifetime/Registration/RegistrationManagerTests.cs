using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Lifetime.Registrations
{
    [TestClass]
    public class RegistrationManagerTests
    {
        [TestMethod]
        public void Baseline()
        {
            // Arrange
            var manager = new TransientLifetimeManager();

            // Validate
            Assert.IsNull(manager.Constructor);
            Assert.IsNull(manager.Fields);
            Assert.IsNull(manager.Properties);
            Assert.IsNull(manager.Methods);

            Assert.IsTrue(ReferenceEquals(LifetimeManager.NoValue,
                                          RegistrationManager.NoValue));
        }

        [TestMethod]
        public void CtorTest()
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

        [TestMethod]
        public void InitializerTest()
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

        [TestMethod]
        public void CtorInitializerTest()
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

        [TestMethod]
        public void AddEmptyTest()
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


        [TestMethod]
        public void CtorAddTest()
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


        [TestMethod]
        public void CtorInitializerAddTest()
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

        [TestMethod]
        public void AddRangeEmptyTest()
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


        [TestMethod]
        public void CtorAddRangeTest()
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


        [TestMethod]
        public void CtorInitializerAddRangeTest()
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


        [DataTestMethod]
        [DynamicData(nameof(GetManagers), DynamicDataSourceType.Method)]
        public void AddInjectionMember(RegistrationManager manager)
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
