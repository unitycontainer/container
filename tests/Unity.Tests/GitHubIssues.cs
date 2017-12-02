using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Microsoft.Practices.Unity.TestSupport;
using Unity;
using Unity.Attributes;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Tests.TestObjects;
using UnityContainer = Unity.UnityContainer;

namespace GitHub
{
    [TestClass]
    public class Issues
    {
        [TestMethod]
        public void unitycontainer_unity_164()
        {
            var container = new UnityContainer();

            container.RegisterType<ILogger, MockLogger>();
            var foo2 = new MockLogger();

            container.RegisterType<ILogger>(new InjectionFactory(x => foo2));
            var result = container.Resolve<ILogger>();

            Assert.AreSame(result, foo2);
        }


        [TestMethod]
        public void unity_156()
        {
            using (var container = new UnityContainer())
            {
                var td = new MockLogger();

                container.RegisterType<MockLogger>(new ContainerControlledLifetimeManager(), new InjectionFactory(_ => td));
                container.RegisterType<ILogger, MockLogger>();

                Assert.AreSame(td, container.Resolve<ILogger>());
                Assert.AreSame(td, container.Resolve<MockLogger>());
            }
            using (var container = new UnityContainer())
            {
                var td = new MockLogger();

                container.RegisterType<MockLogger>(new ContainerControlledLifetimeManager(), new InjectionFactory(_ => td));
                container.RegisterType<ILogger, MockLogger>();

                Assert.AreSame(td, container.Resolve<MockLogger>());
                Assert.AreSame(td, container.Resolve<ILogger>());
            }
        }


        [TestMethod]
        public void unity_154_5()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<OtherEmailService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IService, OtherEmailService>();
            container.RegisterType<IOtherService, OtherEmailService>(new InjectionConstructor(container));

            Assert.AreNotSame(container.Resolve<IService>(),
                              container.Resolve<IOtherService>());
        }


        [TestMethod]
        public void unity_154()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<OtherEmailService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IService, OtherEmailService>();
            container.RegisterType<IOtherService, OtherEmailService>();

            Assert.AreSame(container.Resolve<IService>(),
                           container.Resolve<IOtherService>());
        }


        [TestMethod]
        public void unity_153()
        {
            IUnityContainer rootContainer = new UnityContainer();
            rootContainer.RegisterType<ILogger, MockLogger>(new HierarchicalLifetimeManager());

            using (IUnityContainer childContainer = rootContainer.CreateChildContainer())
            {
                var a = childContainer.Resolve<ILogger>();
                var b = childContainer.Resolve<ILogger>();

                Assert.AreSame(a, b);
            }
        }


        [TestMethod]
        public void Issue_35()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterType<ILogger, MockLogger>(new ContainerControlledLifetimeManager());
            ILogger logger = container.Resolve<ILogger>();

            Assert.IsNotNull(logger);
            Assert.AreSame(container.Resolve<ILogger>(), logger);

            container.RegisterType<MockLogger>(new TransientLifetimeManager());

            Assert.AreSame(container.Resolve<ILogger>(), logger);
        }

        [TestMethod]    
        public void GitHub_Issue_88()   // https://github.com/unitycontainer/unity/issues/88
        {
            using (var unityContainer = new UnityContainer())
            {
                unityContainer.RegisterInstance(true);
                unityContainer.RegisterInstance("true", true);
                unityContainer.RegisterInstance("false", false);

                var resolveAll = unityContainer.ResolveAll(typeof(bool));
                var arr = resolveAll.Select(o => o.ToString()).ToArray();
            }
        }

        [TestMethod]    
        public void GitHub_Issue_54()   // https://github.com/unitycontainer/unity/issues/54
        {
            using (IUnityContainer container = new UnityContainer())
            {
                container.RegisterType(typeof(ITestClass), typeof(TestClass));
                container.RegisterInstance(new TestClass());
                var instance = container.Resolve<ITestClass>(); //0
                Assert.IsNotNull(instance);
            }

            using (IUnityContainer container = new UnityContainer())
            {
                container.RegisterType(typeof(ITestClass), typeof(TestClass));
                container.RegisterType<TestClass>(new ContainerControlledLifetimeManager());

                try
                {
                    var instance = container.Resolve<ITestClass>(); //2
                    Assert.IsNull(instance, "Should threw an exception");
                }
                catch (ResolutionFailedException e)
                {
                    Assert.IsInstanceOfType(e, typeof(ResolutionFailedException));
                }

            }
        }

        [TestMethod]
        public void GitHub_Issue_35_ConflictTypeMapping()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterType<ILogger, MockLogger>(new ContainerControlledLifetimeManager());
            ILogger logger = container.Resolve<ILogger>();

            Assert.IsNotNull(logger);
            Assert.AreSame(container.Resolve<ILogger>(), logger);

            container.RegisterType<MockLogger>(new TransientLifetimeManager());

            Assert.AreSame(container.Resolve<ILogger>(), logger);
        }

        // Test types 
        public interface ITestClass
        { }

        public class TestClass : ITestClass
        {
            public TestClass()
            { }

            [InjectionConstructor]
            public TestClass(TestClass x) //1
            { }
        }
    }
}
