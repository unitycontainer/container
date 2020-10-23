using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Unity;
using Unity.Lifetime;

namespace Container.Scope
{
    public partial class EnumeratorTests
    {
        [TestMethod]
        public void Enumerator_Empty()
        {
            // Act
            var enumerator = Scope.GetEnumerator();

            // Validate
            Assert.IsFalse(enumerator.MoveNext());
        }

        [TestMethod]
        public void Enumerator_Root()
        {
            // Arrange
            var manager = new Unity.Container.ContainerLifetimeManager(this);
            Scope.Add(typeof(IUnityContainer),      manager, true);
            Scope.Add(typeof(IUnityContainerAsync), manager, true);
            Scope.Add(typeof(IServiceProvider),     manager, true);

            // Act
            var enumerator = Scope.GetEnumerator();

            // Validate
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsFalse(enumerator.MoveNext());
        }

        [TestMethod]
        public void Enumerator_Root_Child()
        {
            // Arrange
            var manager = new Unity.Container.ContainerLifetimeManager(this);
            Scope.Add(typeof(IUnityContainer),      manager, true);
            Scope.Add(typeof(IUnityContainerAsync), manager, true);
            Scope.Add(typeof(IServiceProvider),     manager, true);

            var scope = Scope.CreateChildScope(3);
            manager = new Unity.Container.ContainerLifetimeManager(this);
            scope.Add(typeof(IUnityContainer),      manager, true);
            scope.Add(typeof(IUnityContainerAsync), manager, true);
            scope.Add(typeof(IServiceProvider),     manager, true);

            // Act
            var enumerator = scope.GetEnumerator();

            // Validate
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsFalse(enumerator.MoveNext());
        }


        [TestMethod]
        public void Enumerator_EmptyMultilevel()
        {
            var scope = Scope.CreateChildScope(1)
                             .CreateChildScope(3)
                             .CreateChildScope(5);

            // Act
            var enumerator = scope.GetEnumerator();

            // Validate
            Assert.IsFalse(enumerator.MoveNext());
        }

        [TestMethod]
        public void Enumerator_Anonymous_One()
        {
            // Arrange
            Scope.Add(typeof(List<>), Manager);

            // Act
            var enumerator = Scope.GetEnumerator();

            // Validate
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsFalse(enumerator.MoveNext());
        }
        
        [TestMethod]
        public void Enumerator_Anonymous_Two()
        {
            // Arrange
            Scope.Add(typeof(List<>), Manager);
            Scope.Add(typeof(List<>), Manager);

            // Act
            var enumerator = Scope.GetEnumerator();

            // Validate
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsFalse(enumerator.MoveNext());
        }

        [TestMethod]
        public void Enumerator_One_Named()
        {
            // Arrange
            Scope.Add(typeof(List<>), Name, Manager);

            // Act
            var enumerator = Scope.GetEnumerator();

            // Validate
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsFalse(enumerator.MoveNext());
        }

        [TestMethod]
        public void Enumerator_Two_Named()
        {
            // Arrange
            Scope.Add(typeof(List<>), "one", Manager);
            var scope = Scope.CreateChildScope(3);
            scope.Add(typeof(List<>), "two", Manager);

            // Act
            var enumerator = scope.GetEnumerator();

            // Validate
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsFalse(enumerator.MoveNext());
        }

        [TestMethod]
        public void Enumerator_TwoTest()
        {
            // Arrange
            var other = new ContainerControlledLifetimeManager
            {
                Data = new object(),
                Category = RegistrationCategory.Instance
            };
            Scope.Add(typeof(List<>), Manager);
            Scope.Add(typeof(List<>), other);

            // Act
            var enumerator = Scope.GetEnumerator();

            // Validate
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsFalse(enumerator.MoveNext());
        }

        [TestMethod]
        public void Enumerator_TwoEmptyTest()
        {
            // Arrange
            var one = new ContainerControlledLifetimeManager
            {
                Data = "One",
                Category = RegistrationCategory.Instance
            };
            var two = new ContainerControlledLifetimeManager
            {
                Data = "Two",
                Category = RegistrationCategory.Instance
            };

            Scope.Add(typeof(List<>), one);
            Scope.Add(typeof(List<>), two);
            var scope = Scope.CreateChildScope(3);

            // Act
            var enumerator = scope.GetEnumerator();

            // Validate
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsFalse(enumerator.MoveNext());
        }

        [TestMethod]
        public void Enumerator_ThreeTest()
        {
            // Arrange
            var one = new ContainerControlledLifetimeManager
            {
                Data = "One",
                Category = RegistrationCategory.Instance
            };
            var two = new ContainerControlledLifetimeManager
            {
                Data = "Two",
                Category = RegistrationCategory.Instance
            };

            Scope.Add(typeof(List<>), one);
            Scope.Add(typeof(List<>), two);
            var scope = Scope.CreateChildScope(3);
            scope.Add(typeof(List<>), Manager);

            // Act
            var enumerator = scope.GetEnumerator();

            // Validate
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsFalse(enumerator.MoveNext());
        }

    }
}
