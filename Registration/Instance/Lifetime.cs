using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Lifetime;
#endif

namespace Registration
{
    public partial class Instance
    {
        [TestMethod]
        public void SingletonAtRoot()
        {
            // Arrange
            var service = Unresolvable.Create("SingletonAtRoot");

            var child1 = Container.CreateChildContainer();
            var child2 = child1.CreateChildContainer();

            Container.RegisterInstance(typeof(IUnresolvable), null, service, new ContainerControlledLifetimeManager());


            // Act/Verify

            Assert.AreSame(service, Container.Resolve<IUnresolvable>());
            Assert.AreSame(service, child1.Resolve<IUnresolvable>());
            Assert.AreSame(service, child2.Resolve<IUnresolvable>());
        }

        [TestMethod]
        public void SingletonAtChild()
        {
            // Arrange
            var root = Unresolvable.Create("SingletonAtRoot");
            var service = Unresolvable.Create("SingletonAtChild");

            var child1 = Container.CreateChildContainer();
            var child2 = child1.CreateChildContainer();

            Container.RegisterInstance(typeof(IUnresolvable), null, root, new ContainerControlledLifetimeManager());
            child1.RegisterInstance(typeof(IUnresolvable), null, service, new ContainerControlledLifetimeManager());


            // Act/Verify

            Assert.AreSame(root, Container.Resolve<IUnresolvable>());
            Assert.AreSame(service, child1.Resolve<IUnresolvable>());
            Assert.AreSame(service, child2.Resolve<IUnresolvable>());
            Assert.AreNotSame(Container.Resolve<IUnresolvable>(), child2.Resolve<IUnresolvable>());
        }

        [TestMethod]
        public void PerContainerAtRoot()
        {
            // Arrange
            var service = Unresolvable.Create("PerContainerAtRoot");

            var child1 = Container.CreateChildContainer();
            var child2 = child1.CreateChildContainer();

            Container.RegisterInstance(typeof(IUnresolvable), null, service, new ContainerControlledLifetimeManager());


            // Act/Verify

            Assert.AreSame(service, Container.Resolve<IUnresolvable>());
            Assert.AreSame(service, child1.Resolve<IUnresolvable>());
            Assert.AreSame(service, child2.Resolve<IUnresolvable>());
        }

        [TestMethod]
        public void PerContainerAtChild()
        {
            // Arrange
            var service = Unresolvable.Create("PerContainerAtChild");

            var child1 = Container.CreateChildContainer();
            var child2 = child1.CreateChildContainer();

            Container.RegisterInstance(typeof(IUnresolvable), null, Unresolvable.Create("1"), new ContainerControlledLifetimeManager());
            child1.RegisterInstance(typeof(IUnresolvable), null, Unresolvable.Create("2"), new ContainerControlledLifetimeManager());
            child2.RegisterInstance(typeof(IUnresolvable), null, Unresolvable.Create("3"), new ContainerControlledLifetimeManager());


            // Act/Verify

            Assert.AreNotSame(service, Container.Resolve<IUnresolvable>());
            Assert.AreNotSame(service, child1.Resolve<IUnresolvable>());
            Assert.AreNotSame(service, child2.Resolve<IUnresolvable>());
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void PerContainerThrows()
        {
            // Arrange
            var child1 = Container.CreateChildContainer();

            child1.RegisterInstance(typeof(IUnresolvable), null, Unresolvable.Create("PerContainerThrows"), new ContainerControlledLifetimeManager());

            // Act/Verify
            var result = Container.Resolve<IUnresolvable>();
        }


        [TestMethod]
        public void ExternalAtRoot()
        {
            // Arrange
            var service = Unresolvable.Create("ExternalAtRoot");

            var child1 = Container.CreateChildContainer();
            var child2 = child1.CreateChildContainer();

            Container.RegisterInstance(typeof(IUnresolvable), null, service, new ExternallyControlledLifetimeManager());


            // Act/Verify

            Assert.AreSame(service, Container.Resolve<IUnresolvable>());
            Assert.AreSame(service, child1.Resolve<IUnresolvable>());
            Assert.AreSame(service, child2.Resolve<IUnresolvable>());
        }

        [TestMethod]
        public void ExternalAtChild()
        {
            // Arrange
            var service = Unresolvable.Create("ExternalAtChild");

            var child1 = Container.CreateChildContainer();
            var child2 = child1.CreateChildContainer();

            Container.RegisterInstance(typeof(IUnresolvable), null, Unresolvable.Create("1"), new ExternallyControlledLifetimeManager());
            child1.RegisterInstance(typeof(IUnresolvable), null, Unresolvable.Create("2"), new ExternallyControlledLifetimeManager());
            child2.RegisterInstance(typeof(IUnresolvable), null, Unresolvable.Create("3"), new ExternallyControlledLifetimeManager());


            // Act/Verify

            Assert.AreNotSame(service, Container.Resolve<IUnresolvable>());
            Assert.AreNotSame(service, child1.Resolve<IUnresolvable>());
            Assert.AreNotSame(service, child2.Resolve<IUnresolvable>());
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void ExternalThrows()
        {
            // Arrange
            var child1 = Container.CreateChildContainer();

            child1.RegisterInstance(typeof(IUnresolvable), null, Unresolvable.Create("ExternalThrows"), new ExternallyControlledLifetimeManager());

            // Act/Verify
            var result = Container.Resolve<IUnresolvable>();
        }
    }
}
