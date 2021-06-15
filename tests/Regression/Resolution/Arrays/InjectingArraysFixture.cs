using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Injection;
#endif

namespace Resolution
{
    public partial class Arrays
    {
        [TestMethod, TestProperty(RESOLVING, REGISTERED)]
        public void CanConfigureContainerToCallConstructorWithArrayParameter()
        {
            // Arrange
            IService o1 = new Service();
            IService o2 = new OtherService();

            Container.RegisterType<TypeWithArrayParameter>( new InjectionConstructor(typeof(IService[])))
                     .RegisterInstance<IService>("o1", o1)
                     .RegisterInstance<IService>("o2", o2);

            // Act
            var resolved = Container.Resolve<TypeWithArrayParameter>();

            Assert.IsNotNull(resolved.Loggers);
            Assert.AreEqual(2, resolved.Loggers.Length);
            Assert.AreSame(o1, resolved.Loggers[0]);
            Assert.AreSame(o2, resolved.Loggers[1]);
        }

        [TestMethod, TestProperty(RESOLVING, REGISTERED)]
        public void CanConfigureContainerToCallConstructorWithArrayParameterWithNonGenericVersion()
        {
            // Arrange
            IService o1 = new Service();
            IService o2 = new OtherService();

            Container.RegisterType<TypeWithArrayParameter>(new InjectionConstructor(typeof(IService[])))
                     .RegisterInstance<IService>("o1", o1)
                     .RegisterInstance<IService>("o2", o2);

            // Act
            var resolved = Container.Resolve<TypeWithArrayParameter>();

            Assert.IsNotNull(resolved.Loggers);
            Assert.AreEqual(2, resolved.Loggers.Length);
            Assert.AreSame(o1, resolved.Loggers[0]);
            Assert.AreSame(o2, resolved.Loggers[1]);
        }

        [TestMethod, TestProperty(RESOLVING, REGISTERED)]
        public void CanConfigureContainerToInjectSpecificValuesIntoAnArray()
        {
            // Arrange
            IService logger2 = new OtherService();

            Container.RegisterType<TypeWithArrayParameter>(
                        new InjectionConstructor(
                            new ResolvedArrayParameter<IService>(
                                new ResolvedParameter<IService>("log1"),
                                typeof(IService),
                                logger2)))
                     .RegisterType<IService, Service>()
                     .RegisterType<IService, OtherService>("log1");

            // Act
            var result = Container.Resolve<TypeWithArrayParameter>();

            Assert.AreEqual(3, result.Loggers.Length);
            Assert.IsInstanceOfType(result.Loggers[0], typeof(OtherService));
            Assert.IsInstanceOfType(result.Loggers[1], typeof(Service));
            Assert.AreSame(logger2, result.Loggers[2]);
        }

        [TestMethod, TestProperty(RESOLVING, REGISTERED)]
        public void CanConfigureContainerToInjectSpecificValuesIntoAnArrayWithNonGenericVersion()
        {
            // Arrange
            IService logger2 = new OtherService();

            Container.RegisterType<TypeWithArrayParameter>(
                        new InjectionConstructor(
                            new ResolvedArrayParameter(
                                typeof(IService),
                                new ResolvedParameter<IService>("log1"),
                                typeof(IService),
                                logger2)))
                     .RegisterType<IService, Service>()
                     .RegisterType<IService, OtherService>("log1");

            // Act
            var result = Container.Resolve<TypeWithArrayParameter>();

            Assert.AreEqual(3, result.Loggers.Length);
            Assert.IsInstanceOfType(result.Loggers[0], typeof(OtherService));
            Assert.IsInstanceOfType(result.Loggers[1], typeof(Service));
            Assert.AreSame(logger2, result.Loggers[2]);
        }

        [TestMethod, TestProperty(RESOLVING, REGISTERED)]
        public void ContainerAutomaticallyResolvesAllWhenInjectingArrays()
        {
            // Arrange
            IService[] expected = new IService[] { new Service(), new OtherService() };
            Container.RegisterInstance<IService>("one", expected[0])
                     .RegisterInstance<IService>("two", expected[1]);

            var result = Container.Resolve<TypeWithArrayParameter>();


            // Validate
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Loggers.Length);
            Assert.AreSame(expected[0], result.Loggers[0]);
            Assert.AreSame(expected[1], result.Loggers[1]);
        }
    }
}
