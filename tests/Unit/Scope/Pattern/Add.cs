using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Lifetime;

namespace Container.Scope
{
    public partial class ScopeTests
    {
        [TestMethod, TestProperty(TESTING_IUC, TRAIT_ADD)]
        public void RegisterType()
        {
            // Act
            Scope.Register(typeof(ScopeTests), null, new ContainerControlledLifetimeManager());

            // Validate
            Assert.AreEqual(1, Scope.Version);
            Assert.AreEqual(1, Scope.Count);
            Assert.AreEqual(1, Scope.ToArray().Length);
        }

        [TestMethod, TestProperty(TESTING_IUC, TRAIT_ADD)]
        public void RegisterSameType()
        {
            // Act
            Scope.Register(typeof(ScopeTests), null, new ContainerControlledLifetimeManager());
            Scope.Register(typeof(ScopeTests), null, new ContainerControlledLifetimeManager());

            // Validate
            Assert.AreEqual(2, Scope.Version);
            Assert.AreEqual(2, Scope.Count);
            Assert.AreEqual(2, Scope.ToArray().Length);
        }

        [TestMethod, TestProperty(TESTING_IUC, TRAIT_ADD)]
        public void RegisterTypeWithName()
        {
            // Act
            Scope.Register(typeof(ScopeTests), Name, new ContainerControlledLifetimeManager());

            // Validate
            Assert.AreEqual(1, Scope.Version);
            Assert.AreEqual(2, Scope.Count);
            Assert.AreEqual(2, Scope.ToArray().Length);
        }

        [TestMethod, TestProperty(TESTING_IUC, TRAIT_ADD)]
        public void RegisterTypeWithSameNameTwice()
        {
            // Act
            Scope.Register(typeof(ScopeTests), Name, new ContainerControlledLifetimeManager());
            Scope.Register(typeof(ScopeTests), Name, new ContainerControlledLifetimeManager());

            // Validate
            Assert.AreEqual(1, Scope.Version);
            Assert.AreEqual(1, Scope.Count);
            Assert.AreEqual(1, Scope.ToArray().Length);
        }
    }
}
