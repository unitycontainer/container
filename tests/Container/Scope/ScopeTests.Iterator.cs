using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Unity;
using Unity.Container;
using Unity.Lifetime;
using Unity.Storage;
using static Unity.Container.Scope;

namespace Container.Scopes
{
    public partial class ScopeTests
    {
        [TestMethod]
        public void Iterator_One_Anonymous()
        {
            // Arrange
            Scope.Add(typeof(List<>), Manager);
            Span<Metadata> buffer = stackalloc Metadata[Scope.Level + 1];
            var references = Scope.GetReferences(typeof(List<>), in buffer);

            // Act
            var iterator = new Enumerator(Scope, typeof(List<>));

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
            Span<Metadata> buffer = stackalloc Metadata[Scope.Level + 1];
            var references = Scope.GetReferences(typeof(List<>), in buffer);

            // Act
            var iterator = new Enumerator(Scope, typeof(List<>));

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

            Span<Metadata> buffer = stackalloc Metadata[Scope.Level + 1];
            var references = Scope.GetReferences(typeof(List<>), in buffer);

            // Act
            var iterator = new Enumerator(Scope, typeof(List<>));

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
            
            Span<Metadata> buffer = stackalloc Metadata[scope.Level + 1];
            var references = scope.GetReferences(typeof(List<>), in buffer);

            // Act
            var iterator = new Enumerator(scope, typeof(List<>));

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

            Span<Metadata> buffer = stackalloc Metadata[scope.Level + 1];
            var references = scope.GetReferences(typeof(List<>), in buffer);

            // Act
            var iterator = new Enumerator(scope, typeof(List<>));

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
