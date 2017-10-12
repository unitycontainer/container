using System;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Exceptions;
using Unity.Injection;

namespace Unity.Tests
{
    [TestClass]
    public class DelegateInjectionFactoryFixture
    {
        [TestMethod]
        public void DelegateFactory_NoArguments()
        {
            using (IUnityContainer container = new UnityContainer())
            {
                MockLogger Factory() => new MockLogger();

                container.RegisterType<ILogger>(new DelegateInjectionFactory((Func<MockLogger>) Factory));

                Assert.IsNotNull(container.Resolve<ILogger>());
            }

        }

        [TestMethod]
        public void DelegateFactory_PassContainer()
        {
            using (IUnityContainer container = new UnityContainer())
            {
                MockLogger Factory(IUnityContainer unity)
                {
                    Assert.IsNotNull(unity);
                    Assert.IsInstanceOfType(unity, typeof(IUnityContainer));
                    return new MockLogger();
                }

                container.RegisterType<ILogger>(new DelegateInjectionFactory((Func<IUnityContainer, MockLogger>) Factory));

                Assert.IsNotNull(container.Resolve<ILogger>());
            }

        }

        [TestMethod]
        public void DelegateFactory_PassType()
        {
            using (IUnityContainer container = new UnityContainer())
            {
                MockLogger Factory(Type type) // Special case
                {
                    Assert.IsNotNull(type);
                    Assert.IsInstanceOfType(type, typeof(Type));
                    Assert.AreEqual(type, typeof(ILogger));
                    return new MockLogger();
                }

                container.RegisterType<ILogger>(new DelegateInjectionFactory((Func<Type, MockLogger>)Factory));

                Assert.IsNotNull(container.Resolve<ILogger>());
            }

        }

        [TestMethod]
        public void DelegateFactory_PassTypeAndName()
        {
            var regName = "arbitrary-name";

            using (IUnityContainer container = new UnityContainer())
            {
                MockLogger Factory(Type type, string name) // Special case
                {
                    Assert.IsNotNull(type);
                    Assert.IsInstanceOfType(type, typeof(Type));
                    Assert.AreEqual(type, typeof(ILogger));
                    Assert.AreEqual(name, regName);
                    return new MockLogger();
                }

                container.RegisterType<ILogger>(regName, new DelegateInjectionFactory((Func<Type, string, MockLogger>)Factory));

                Assert.IsNotNull(container.Resolve<ILogger>(regName));
            }

        }

        [TestMethod]
        public void DelegateFactory_PassObject()
        {
            using (IUnityContainer container = new UnityContainer())
            {
                MockLogger Factory(object obj) // Special case
                {
                    Assert.IsNotNull(obj);
                    Assert.IsInstanceOfType(obj, typeof(object));
                    return new MockLogger();
                }

                container.RegisterType<ILogger>(new DelegateInjectionFactory((Func<object, MockLogger>)Factory));

                Assert.IsNotNull(container.Resolve<ILogger>());
            }

        }


        [TestMethod]
        public void DelegateFactory_PassInt()
        {
            using (IUnityContainer container = new UnityContainer())
            {
                MockLogger Factory(int obj) 
                {
                    Assert.IsNotNull(obj);
                    Assert.IsInstanceOfType(obj, typeof(int));
                    Assert.AreEqual(obj, 0);
                    return new MockLogger();
                }

                container.RegisterType<ILogger>(new DelegateInjectionFactory((Func<int, MockLogger>)Factory));
                try
                {
                    Assert.IsNotNull(container.Resolve<ILogger>());
                    Assert.Fail();
                }
                catch (Exception e)
                {
                    Assert.IsInstanceOfType(e, typeof(ResolutionFailedException));
                }
            }

        }

    }
}
