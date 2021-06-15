using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Injection;
using Unity.Lifetime;
#endif

namespace Resolution
{
    public partial class Generics
    {
        [TestMethod]
        public void CanSpecializeGenericTypes()
        {
            Container.RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>));
            ICommand<Service> cmd = Container.Resolve<ICommand<Service>>();
            Assert.IsInstanceOfType(cmd, typeof(ConcreteCommand<Service>));
        }

        [TestMethod]
        public void ConfiguringConstructorThatTakesOpenGenericTypeDoesNotThrow()
        {
            Container.RegisterType(typeof(LoggingCommand<>),
                    new InjectionConstructor(new ResolvedParameter(typeof(ICommand<>), "concrete")));
        }

        [TestMethod]
        public void CanConfigureGenericMethodInjectionInContainer()
        {
            Container.RegisterType(typeof(ICommand<>), typeof(LoggingCommand<>),
                    new InjectionConstructor(new ResolvedParameter(typeof(ICommand<>), "concrete")),
                    new InjectionMethod("ChainedExecute", new ResolvedParameter(typeof(ICommand<>), "inner")))
                .RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>), "concrete")
                .RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>), "inner");
        }

        [TestMethod]
        public void CanConfigureInjectionForNonGenericMethodOnGenericClass()
        {
            Container.RegisterType(typeof(ICommand<>), typeof(LoggingCommand<>),
                new InjectionConstructor(),
                new InjectionMethod("InjectMe"));

            ICommand<Service> result = Container.Resolve<ICommand<Service>>();
            LoggingCommand<Service> logResult = (LoggingCommand<Service>)result;

            Assert.IsTrue(logResult.WasInjected);
        }

        [TestMethod]
        public void CanCallDefaultConstructorOnGeneric()
        {
            Container.RegisterType(typeof(ICommand<>), typeof(LoggingCommand<>),  new InjectionConstructor())
                     .RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>), "inner");

            ICommand<Service> result = Container.Resolve<ICommand<Service>>();

            Assert.IsInstanceOfType(result, typeof(LoggingCommand<Service>));
        }

        [TestMethod]
        public void CanConfigureInjectionForGenericProperty()
        {
            Container.RegisterType(typeof(ICommand<>), typeof(LoggingCommand<>),
                    new InjectionConstructor(),
                    new InjectionProperty("Inner",
                        new ResolvedParameter(typeof(ICommand<>), "inner")))
                .RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>), "inner");
        }

        [TestMethod]
        public void CanInjectNonGenericPropertyOnGenericClass()
        {
            Container.RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>),
                    new InjectionProperty("NonGenericProperty"));

            ConcreteCommand<Service> result = (ConcreteCommand<Service>)(Container.Resolve<ICommand<Service>>());
            Assert.IsNotNull(result.NonGenericProperty);
        }

        [TestMethod]
        public void ContainerControlledOpenGenericsAreDisposed()
        {
            Container.RegisterType(typeof(ICommand<>), typeof(DisposableCommand<>), new ContainerControlledLifetimeManager());

            var accountCommand = Container.Resolve<ICommand<Service>>();
            var userCommand = Container.Resolve<ICommand<OtherService>>();

            Container.Dispose();

            Assert.IsTrue(((DisposableCommand<Service>)accountCommand).Disposed);
            Assert.IsTrue(((DisposableCommand<OtherService>)userCommand).Disposed);
        }
    }
}

