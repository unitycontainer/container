using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;
using Unity.Lifetime;

namespace Container.Scope
{
    public partial class ScopeTests
    {
        [TestMethod, TestProperty(TESTING, TRAIT_ADD)]
        public void AddEmptySpanTest()
        {
            ReadOnlySpan<RegistrationDescriptor> span = new ReadOnlySpan<RegistrationDescriptor>();

            // Act
            Scope.Register(in span);

            // Validate
            Assert.AreEqual(0, Scope.Version);
            Assert.AreEqual(0, Scope.Count);
            Assert.AreEqual(0, Scope.ToArray().Length);
        }

        [TestMethod, TestProperty(TESTING, TRAIT_ADD)]
        [ExpectedException(typeof(ArgumentException))]
        public void AddUninitializedManagerTest()
        {
            // Arrange
            ReadOnlySpan<RegistrationDescriptor> array = new[] 
            { 
                new RegistrationDescriptor(new ContainerControlledLifetimeManager()) 
            };

            // Act
            Scope.Register(array);
        }

        [TestMethod, TestProperty(TESTING, TRAIT_ADD)]
        public void AddTypeManagerTest()
        {
            // Arrange
            ReadOnlySpan<RegistrationDescriptor> array = new[]
            {
                new RegistrationDescriptor(new ContainerControlledLifetimeManager
                { 
                    Data = typeof(ScopeTests),
                    Category = RegistrationCategory.Type
                })
            };

            // Act
            Scope.Register(array);

            // Validate
            Assert.AreEqual(1, Scope.Version);
            Assert.AreEqual(1, Scope.Count);
            Assert.AreEqual(1, Scope.ToArray().Length);
        }

        [DataTestMethod, TestProperty(TESTING, TRAIT_ADD)]
        [DynamicData(nameof(AddSize))]
        public void AddUntillExpandsTest(int size)
        {
            // Arrange
            var manager = new ContainerControlledLifetimeManager
            {
                Data = typeof(ScopeTests),
                Category = RegistrationCategory.Type
            };

            RegistrationDescriptor[] array = new RegistrationDescriptor[size];

            for (int i = 0; i < size; i++)
                array[i] = new RegistrationDescriptor(manager);

            // Act
            Scope.Register(array);

            // Validate
            Assert.AreNotEqual(0, Scope.ToArray().Length);
        }


        [TestMethod, TestProperty(TESTING, TRAIT_ADD)]
        public void AddAllTest()
        {
            // Arrange
            ReadOnlySpan<RegistrationDescriptor> span = Registrations;

            // Act
            Scope.Register(span);

            // Validate
            Assert.AreEqual(5995, Scope.Version);
            Assert.AreEqual(6348, Scope.Count);
            Assert.AreEqual(6348, Scope.ToArray().Length);
        }


        [TestMethod, TestProperty(TESTING, TRAIT_ADD)]
        public void AddEdgeCasesTest()
        {
            // Arrange
            ReadOnlySpan<RegistrationDescriptor> span = new[]
            {
                new RegistrationDescriptor( Manager, typeof(ScopeTests) ),
                new RegistrationDescriptor( Manager, typeof(ScopeTests), null, Manager.GetType() ),
                new RegistrationDescriptor( Manager, typeof(ScopeTests), null, typeof(string), null )
            };

            // Act
            Scope.Register(span);

            // Validate
            Assert.AreEqual(5, Scope.Version);
            Assert.AreEqual(5, Scope.Count);
            Assert.AreEqual(5, Scope.ToArray().Length);
        }

        [TestMethod, TestProperty(TESTING, TRAIT_ADD)]
        public void AddNamedTest()
        {
            var manager = new ContainerControlledLifetimeManager
            {
                Data = new object(),
                Category = RegistrationCategory.Instance
            };

            // Arrange
            ReadOnlySpan<RegistrationDescriptor> span = new[]
            {
                new RegistrationDescriptor( "1",  manager ),
                new RegistrationDescriptor( "2",  manager ),
                new RegistrationDescriptor( null, manager )
            };

            // Act
            Scope.Register(span);

            // Validate
            Assert.AreEqual(3, Scope.Version);
            Assert.AreEqual(3, Scope.Count);
            Assert.AreEqual(3, Scope.ToArray().Length);
        }

        [TestMethod, TestProperty(TESTING, TRAIT_ADD)]
        public void AddNamedEdgeCasesTest()
        {
            ReadOnlySpan<RegistrationDescriptor> span = new[]
            {
                new RegistrationDescriptor( Name, Manager, typeof(ScopeTests) ),
                new RegistrationDescriptor( Name, Manager, typeof(ScopeTests), null, Manager.GetType() ),
                new RegistrationDescriptor( Name, Manager, typeof(ScopeTests), null, typeof(string), null )
            };

            // Act
            Scope.Register(span);

            // Validate
            Assert.AreEqual(5, Scope.Version);
            Assert.AreEqual(6, Scope.Count);
            Assert.AreEqual(6, Scope.ToArray().Length);
        }

        [DataTestMethod, TestProperty(TESTING, TRAIT_ADD)]
        [DynamicData(nameof(AddSize))]
        public void AddManagerExpandsTest(int size)
        {
            ReadOnlySpan<RegistrationDescriptor> span = Registrations;

            // Act
            Scope.Register(span.Slice(0, size));

            // Validate
            Assert.AreNotEqual(0, Scope.ToArray().Length);
        }

        public static IEnumerable<object[]> AddSize => Enumerable.Range(1, 20).Select(n => new object[] { n });
    }
}
