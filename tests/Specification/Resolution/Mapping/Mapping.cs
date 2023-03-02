using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Lifetime;
#endif

namespace Resolution
{
    public partial class Mapping
    {
        public const string Legacy = "legacy";

        [TestMethod]
        public void ServiceItself()
        {
            // Arrange
            var instance = new Service();
            var factory = new Service();

            Container.RegisterType<Service>();
            Container.RegisterInstance(Name, instance, new ContainerControlledLifetimeManager());
#if UNITY_V4
            Container.RegisterType<Service>(Legacy, new InjectionFactory((c, t, n) => factory));
#else
            Container.RegisterFactory<Service>(Legacy, (c, t, n) => factory);
#endif
            // Act
            var service1 = Container.Resolve<Service>();
            var service2 = Container.Resolve<Service>(Name);
            var service3 = Container.Resolve<Service>(Legacy);


            // Assert
            Assert.IsNotNull(service1);
            Assert.IsNotNull(service2);
            Assert.IsNotNull(service3);

            Assert.IsInstanceOfType(service1, typeof(Service));
            Assert.IsInstanceOfType(service2, typeof(Service));
            Assert.IsInstanceOfType(service3, typeof(Service));

            Assert.AreSame(instance, service2);
            Assert.AreSame(factory, service3);
        }

        [TestMethod]
        public void ServiceToImplementation()
        {
            // Arrange
            Container.RegisterType(typeof(IService), typeof(Service));

            // Act
            var service = Container.Resolve<IService>();

            // Assert
            Assert.IsNotNull(service);
            Assert.IsInstanceOfType(service, typeof(Service));
        }

        [TestMethod]
        public void MappingToInstance()
        {
            // Arrange
            var service = new Service();
            Container.RegisterInstance(service);
            Container.RegisterType(typeof(IService), typeof(Service));

            // Act
            var service1 = Container.Resolve<IService>();

            // Assert
            Assert.IsNotNull(service1);
            Assert.AreSame(service, service1);
        }

        [TestMethod]
        public void MappingToInstanceInChild()
        {
            // Arrange
            var service = new Service();
            Container.RegisterInstance(service);
            Container.RegisterType(typeof(IService), typeof(Service));

            // Act
            var service1 = Container.CreateChildContainer()
                                    .Resolve<IService>();
            // Assert
            Assert.IsNotNull(service1);
            Assert.AreSame(service, service1);
        }

        [TestMethod]
        public void MappingOverriddenToInstance()
        {
            // Arrange
            var service = new Service();
            Container.RegisterInstance(service);
            Container.RegisterType(typeof(IService), typeof(Service));

            // Act / Validate
            var service1 = Container.Resolve<IService>();
            Assert.IsNotNull(service1);
            Assert.AreSame(service, service1);

            // Act / Validate
            Container.RegisterInstance(new Service());
            var service2 = Container.Resolve<IService>();
            Assert.IsNotNull(service2);
            Assert.AreNotSame(service1, service2);
        }

        [TestMethod]
        public void Mappings()
        {
            // Arrange
            Container.RegisterType(typeof(Service), new ContainerControlledLifetimeManager());
            Container.RegisterType(typeof(IService1), typeof(Service));
            Container.RegisterType(typeof(IService2), typeof(Service));

            // Act
            var service1 = Container.Resolve<IService1>();
            var service2 = Container.Resolve<IService2>();

            // Assert
            Assert.IsNotNull(service1);
            Assert.IsNotNull(service2);

            Assert.AreSame<object>(service1, service2);
        }

        [TestMethod]
        public void NamedMappedInterfaceInstanceRegistrationCanBeResolved()
        {
            Container.RegisterInstance<IService1>("ATest", new Service());
            var iTest = (IService1)Container.Resolve(typeof(IService1), "ATest");

            Assert.IsNotNull(iTest);
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void ThrowsExceptionOnNagative()
        {
            Container.RegisterType<IService1, IService1>();
            Container.Resolve<IService1>("ATest");
        }

        [TestMethod]
        public void LastReplacesPrevious()
        {
            // Arrange
            Container.RegisterType<IService, Service>();
            Container.RegisterType<IService, OtherService>();

            // Act
            var service = Container.Resolve<IService>();

            // Assert
            Assert.IsNotNull(service);
            Assert.IsInstanceOfType(service, typeof(OtherService));
        }

        [TestMethod]
        public void ScopedServiceCanBeResolved()
        {
            // Arrange
            Container.RegisterType<IService, Service>(new HierarchicalLifetimeManager());

            // Act
            using (var scope = Container.CreateChildContainer())
            {
                var ContainerScopedService = Container.Resolve<IService>();
                var scopedService1 = scope.Resolve<IService>();
                var scopedService2 = scope.Resolve<IService>();

                // Assert
                Assert.AreNotSame(ContainerScopedService, scopedService1);
                Assert.AreSame(scopedService1, scopedService2);
            }
        }

        [TestMethod]
        public void NestedScopedServiceCanBeResolved()
        {
            // Arrange
            Container.RegisterType<IService, Service>(new HierarchicalLifetimeManager());

            // Act
            using (var outerScope = Container.CreateChildContainer())
            using (var innerScope = outerScope.CreateChildContainer())
            {
                var outerScopedService = outerScope.Resolve<IService>();
                var innerScopedService = innerScope.Resolve<IService>();

                // Assert
                Assert.IsNotNull(outerScopedService);
                Assert.IsNotNull(innerScopedService);
                Assert.AreNotSame(outerScopedService, innerScopedService);
            }
        }

        [TestMethod]
        public void SingletonServicesComeFromRootContainer()
        {
            // Arrange
            Container.RegisterType<IService, Service>(new ContainerControlledLifetimeManager());

            Service disposableService1;
            Service disposableService2;

            // Act and Assert
            using (var scope = Container.CreateChildContainer())
            {
                var service = scope.Resolve<IService>();
                disposableService1 = (Service)service;
                Assert.IsFalse(disposableService1.IsDisposed);
            }

            Assert.IsFalse(disposableService1.IsDisposed);

            using (var scope = Container.CreateChildContainer())
            {
                var service = scope.Resolve<IService>();
                disposableService2 = (Service)service;
                Assert.IsFalse(disposableService2.IsDisposed);
            }

            Assert.IsFalse(disposableService2.IsDisposed);
            Assert.AreSame(disposableService1, disposableService2);
        }

        [TestMethod]
        public void NestedScopedServiceCanBeResolvedWithNoFallbackContainer()
        {
            // Arrange
            Container.RegisterType<IService, Service>(new HierarchicalLifetimeManager());
            // Act
            using (var outerScope = Container.CreateChildContainer())
            using (var innerScope = outerScope.CreateChildContainer())
            {
                var outerScopedService = outerScope.Resolve<IService>();
                var innerScopedService = innerScope.Resolve<IService>();

                // Assert
                Assert.AreNotSame(outerScopedService, innerScopedService);
            }
        }



        [TestMethod]
        public void ServicesRegisteredWithImplementationTypeCanBeResolved()
        {
            // Arrange
            Container.RegisterType<IService, Service>();

            // Act
            var service = Container.Resolve<IService>();

            // Assert
            Assert.IsNotNull(service);
            Assert.IsInstanceOfType(service, typeof(Service));
        }

        [TestMethod]
        public void ServicesRegisteredWithImplementationType_ReturnDifferentInstancesPerResolution_ForTransientServices()
        {
            // Arrange
            Container.RegisterType<IService, Service>();

            // Act
            var service1 = Container.Resolve<IService>();
            var service2 = Container.Resolve<IService>();

            // Assert
            Assert.IsInstanceOfType(service1, typeof(Service));
            Assert.IsInstanceOfType(service1, typeof(Service));

            Assert.AreNotSame(service1, service2);
        }

        [TestMethod]
        public void ServicesRegisteredWithImplementationType_ReturnSameInstancesPerResolution_ForSingletons()
        {
            // Arrange
            Container.RegisterType<IService, Service>(new ContainerControlledLifetimeManager());

            // Act
            var service1 = Container.Resolve<IService>();
            var service2 = Container.Resolve<IService>();

            // Assert
            Assert.IsInstanceOfType(service1, typeof(Service));
            Assert.IsInstanceOfType(service1, typeof(Service));

            Assert.AreSame(service1, service2);
        }

        [TestMethod]
        public void ServiceInstanceCanBeResolved()
        {
            // Arrange
            var instance = new Service();
            Container.RegisterInstance<IService>(instance);

            // Act
            var service = Container.Resolve<IService>();

            // Assert
            Assert.AreSame(instance, service);
        }

        [TestMethod]
        public void TransientServiceCanBeResolvedFromContainer()
        {
            // Arrange
            Container.RegisterType<IService, Service>();

            // Act
            var service1 = Container.Resolve<IService>();
            var service2 = Container.Resolve<IService>();

            // Assert
            Assert.IsNotNull(service1);
            Assert.AreNotSame(service1, service2);
        }

        [TestMethod]
        public void TransientServiceCanBeResolvedFromScope()
        {
            // Arrange
            Container.RegisterType<IService, Service>();

            // Act
            var service1 = Container.Resolve<IService>();
            using (var scope = Container.CreateChildContainer())
            {
                var scopedService1 = scope.Resolve<IService>();
                var scopedService2 = scope.Resolve<IService>();

                // Assert
                Assert.AreNotSame(service1, scopedService1);
                Assert.AreNotSame(service1, scopedService2);
                Assert.AreNotSame(scopedService1, scopedService2);
            }
        }

    }
}
