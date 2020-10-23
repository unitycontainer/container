﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Unity;
using Unity.Container;
using Unity.Lifetime;
using static Unity.Container.Scope;

namespace Container.Scopes
{
    public partial class ScopeTests
    {
        [TestMethod]
        public void Iterator_Empty()
        {
            // Act
            Span<Location> buffer = stackalloc Location[Scope.Level + 1];
            var references = Scope.GetReferences(typeof(List<>), in buffer);
            var iterator = new Iterator(Scope, typeof(List<>));

            // Validate
            Assert.IsFalse(iterator.MoveNext(in references));
        }

        [TestMethod]
        public void Iterator_EmptyMultilevel()
        {
            // Arrange
            var scope = Scope.CreateChildScope(1)
                             .CreateChildScope(3)
                             .CreateChildScope(5);
            Span<Location> buffer = stackalloc Location[scope.Level + 1];
            var references = Scope.GetReferences(typeof(List<>), in buffer);

            // Act
            var iterator = new Iterator(Scope, typeof(List<>));

            // Validate
            Assert.IsFalse(iterator.MoveNext(in references));
        }

        [TestMethod]
        public void Iterator_One_Anonymous()
        {
            // Arrange
            Scope.Add(typeof(List<>), Manager);
            Span<Location> buffer = stackalloc Location[Scope.Level + 1];
            var references = Scope.GetReferences(typeof(List<>), in buffer);

            // Act
            var iterator = new Iterator(Scope, typeof(List<>));

            // Validate
            Assert.IsTrue(iterator.MoveNext(in references));
            Assert.AreSame(iterator.Internal.Manager, Manager);
            Assert.IsFalse(iterator.MoveNext(in references));
        }

        [TestMethod]
        public void Iterator_One_Named()
        {
            // Arrange
            Scope.Add(typeof(List<>), Name, Manager);
            Span<Location> buffer = stackalloc Location[Scope.Level + 1];
            var references = Scope.GetReferences(typeof(List<>), in buffer);

            // Act
            var iterator = new Iterator(Scope, typeof(List<>));

            // Validate
            Assert.IsTrue(iterator.MoveNext(in references));
            Assert.IsNull(iterator.Internal.Manager);
            Assert.IsTrue(iterator.MoveNext(in references));
            Assert.AreSame(iterator.Internal.Manager, Manager);
            Assert.IsFalse(iterator.MoveNext(in references));
        }

        [TestMethod]
        public void Iterator_TwoTest()
        {
            // Arrange
            var other = new ContainerControlledLifetimeManager
            {
                Data = new object(),
                Category = RegistrationCategory.Instance
            };
            Scope.Add(typeof(List<>), Manager);
            Scope.Add(typeof(List<>), other);

            Span<Location> buffer = stackalloc Location[Scope.Level + 1];
            var references = Scope.GetReferences(typeof(List<>), in buffer);

            // Act
            var iterator = new Iterator(Scope, typeof(List<>));

            // Validate
            Assert.IsTrue(iterator.MoveNext(in references));
            Assert.AreSame(iterator.Internal.Manager, other);
            Assert.IsTrue(iterator.MoveNext(in references));
            Assert.AreSame(iterator.Internal.Manager, Manager);
            Assert.IsFalse(iterator.MoveNext(in references));
        }

        [TestMethod]
        public void Iterator_TwoEmptyTest()
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
            
            Span<Location> buffer = stackalloc Location[scope.Level + 1];
            var references = scope.GetReferences(typeof(List<>), in buffer);

            // Act
            var iterator = new Iterator(scope, typeof(List<>));

            // Validate
            Assert.IsTrue(iterator.MoveNext(in references));
            Assert.AreSame(iterator.Internal.Manager, two);
            Assert.IsTrue(iterator.MoveNext(in references));
            Assert.AreSame(iterator.Internal.Manager, one);
            Assert.IsFalse(iterator.MoveNext(in references));
        }

        [TestMethod]
        public void Iterator_ThreeTest()
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

            Span<Location> buffer = stackalloc Location[scope.Level + 1];
            var references = scope.GetReferences(typeof(List<>), in buffer);

            // Act
            var iterator = new Iterator(scope, typeof(List<>));

            // Validate
            Assert.IsTrue(iterator.MoveNext(in references));
            Assert.AreSame(iterator.Internal.Manager, Manager);
            Assert.IsTrue(iterator.MoveNext(in references));
            Assert.AreSame(iterator.Internal.Manager, two);
            Assert.IsTrue(iterator.MoveNext(in references));
            Assert.AreSame(iterator.Internal.Manager, one);
            Assert.IsFalse(iterator.MoveNext(in references));
        }

    }
}
