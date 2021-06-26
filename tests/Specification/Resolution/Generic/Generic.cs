using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Resolution
{
    public partial class Generics
    {
        [TestMethod]
        public void GenericService()
        {
            // Arrange
            Container.RegisterType(typeof(Foo<>));

            // Act 
            var instance = Container.Resolve<Foo<Service>>();

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Foo<Service>));
        }

        [TestMethod]
        public void GenericIService()
        {
            // Arrange
            Container.RegisterType(typeof(IService), typeof(Service))
                     .RegisterType(typeof(Foo<>));

            // Act 
            var instance = Container.Resolve<Foo<IService>>();

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Foo<IService>));
        }

        [TestMethod]
        public void GenericConstrained()
        {
            // Act 
            var instance = Container.Resolve<Constrained<Service>>();

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Constrained<Service>));
        }

        [TestMethod]
        public void GenericConstrained_Registered()
        {
            // Arrange
            Container.RegisterType(typeof(Constrained<>));

            // Act 
            var instance = Container.Resolve<Constrained<Service>>();

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Constrained<Service>));
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void GenericConstrained_Violation()
        {
            // Arrange
            Container.RegisterType(typeof(IService), typeof(OtherService));
            Container.RegisterType(typeof(IConstrained<>), typeof(Constrained<>));

            // Act
            var instance = Container.Resolve<IConstrained<IService>>();

            // Assert
            Assert.IsNotNull(instance);
        }
    }
}
