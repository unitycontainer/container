﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity;
using Unity.Lifetime;

namespace Container.Scope
{
    public partial class ScopeTests
    {
        [TestMethod]
        public void Enumerator_Empty()
        {
            // Act
            var enumerator = Scope.GetEnumerator(typeof(List<>));

            // Validate
            Assert.IsFalse(enumerator.MoveNext());
        }

        [TestMethod]
        public void Enumerator_EmptyMultilevel()
        {
            var scope = Scope.CreateChildScope(1)
                             .CreateChildScope(3)
                             .CreateChildScope(5);

            // Act
            var enumerator = scope.GetEnumerator(typeof(List<>));

            // Validate
            Assert.IsFalse(enumerator.MoveNext());
        }

        [TestMethod]
        public void Enumerator_One_Anonymous()
        {
            // Arrange
            Scope.Add(typeof(List<>), Manager);

            // Act
            var enumerator = Scope.GetEnumerator(typeof(List<>));

            // Validate
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(enumerator.Current.Manager, Manager);
            Assert.IsFalse(enumerator.MoveNext());
        }

        [TestMethod]
        public void Enumerator_One_Named()
        {
            // Arrange
            Scope.Add(typeof(List<>), Name, Manager);

            // Act
            var enumerator = Scope.GetEnumerator(typeof(List<>));

            // Validate
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(enumerator.Current.Manager, Manager);
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
            var enumerator = Scope.GetEnumerator(typeof(List<>));

            // Validate
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(enumerator.Current.Manager, other);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(enumerator.Current.Manager, Manager);
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
            var enumerator = scope.GetEnumerator(typeof(List<>));

            // Validate
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(enumerator.Current.Manager, two);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(enumerator.Current.Manager, one);
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
            var enumerator = scope.GetEnumerator(typeof(List<>));

            // Validate
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(enumerator.Current.Manager, Manager);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(enumerator.Current.Manager, two);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreSame(enumerator.Current.Manager, one);
            Assert.IsFalse(enumerator.MoveNext());
        }

    }
}
