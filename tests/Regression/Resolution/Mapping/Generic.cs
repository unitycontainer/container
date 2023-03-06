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
        [TestMethod]
        public void GenericClosed()
        {
            // Arrange
            Container.RegisterInstance(55);
            Container.RegisterType(typeof(Foo<int>), new ContainerControlledLifetimeManager());
            Container.RegisterType(typeof(IFoo1<int>), typeof(Foo<int>));
            Container.RegisterType(typeof(IFoo2<int>), typeof(Foo<int>));

            // Act
            var service1 = Container.Resolve<IFoo1<int>>();
            var service2 = Container.Resolve<IFoo2<int>>();

            // Assert
            Assert.IsNotNull(service1);
            Assert.IsNotNull(service2);

            Assert.AreSame<object>(service1, service2);
        }

        [TestMethod]
        public void GenericOpen()
        {
            // Arrange
            Container.RegisterInstance(55)
                     .RegisterType(typeof(Foo<>), new ContainerControlledLifetimeManager())
                     .RegisterType(typeof(IFoo1<>), typeof(Foo<>))
                     .RegisterType(typeof(IFoo2<>), typeof(Foo<>));

            // Act
            var service1 = Container.Resolve<IFoo1<int>>();
            var service2 = Container.Resolve<IFoo2<int>>();

            // Assert
            Assert.IsNotNull(service1);
            Assert.IsNotNull(service2);

            Assert.AreSame<object>(service1, service2);
        }

        [TestMethod]
        public void OpenGenericServicesCanBeResolved()
        {
            // Arrange
            Container.RegisterType<IService, Service>(new ContainerControlledLifetimeManager());
            Container.RegisterType(typeof(IFoo<>), typeof(Foo<>));

            // Act
            var genericService = Container.Resolve<IFoo<IService>>();
            var singletonService = Container.Resolve<IService>();

            // Assert
            Assert.AreSame(singletonService, genericService.Value);
        }

        [TestMethod]
        public void ClosedServicesPreferredOverOpenGenericServices()
        {
            // Arrange
            Container.RegisterType<IService, Service>();
            Container.RegisterType(typeof(IFoo<>), typeof(Foo<>));
            Container.RegisterType(typeof(IFoo<IService>), typeof(Foo<IService>));

            // Act
            var service = Container.Resolve<IFoo<IService>>();

            // Assert
            Assert.IsInstanceOfType(service.Value, typeof(Service));
        }

        [TestMethod]
        public void Maping_UnnamedPicksUnnamedArg()
        {
            // Arrange
            Container.RegisterType<IService, Service>();
            Container.RegisterType<IService, OtherService>(Name);
            Container.RegisterType(typeof(IFoo<>), typeof(Foo<>));

            // Act 
            var instance = Container.Resolve<IFoo<IService>>();

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(IFoo<IService>));
            Assert.IsInstanceOfType(instance.Value, typeof(Service));
        }

        [TestMethod]
        public void Maping_NamedPicksUnnamedArg()
        {
            // Arrange
            Container.RegisterType<IService, Service>();
            Container.RegisterType<IService, OtherService>(Name);
            Container.RegisterType(typeof(IFoo<>), typeof(Foo<>), Name);

            // Act 
            var instance = Container.Resolve<IFoo<IService>>(Name);

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(IFoo<IService>));
            Assert.IsInstanceOfType(instance.Value, typeof(Service));
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void Mapping_NonMatchingArgIsIgnored()
        {
            // Arrange
            Container.RegisterType(typeof(IFoo<>), typeof(Foo<>));
            Container.RegisterType<IService, OtherService>(Name);

            // Act 
            var instance = Container.Resolve<IFoo<IService>>();

            // Assert
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void HandlesConstraint()
        {
            // Arrange
            Container.RegisterType(typeof(IConstrained<>), typeof(Constrained<>));

            // Act
            var instance = Container.Resolve<IConstrained<Service>>();

            // Assert
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void HandlesConstraintViolation()
        {
            // Arrange
            Container.RegisterType(typeof(IService), typeof(Service));
            Container.RegisterType(typeof(IConstrained<>), typeof(Constrained<>));

            // After mapping creates invalid type: Constrained<IService>
            var instance = Container.Resolve<IConstrained<IService>>();

            // Assert
            Assert.IsNotNull(instance);
        }
    }
}
