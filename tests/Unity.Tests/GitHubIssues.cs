using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Microsoft.Practices.Unity.TestSupport;
using Unity;
using Unity.Attributes;
using Unity.Exceptions;
using Unity.Lifetime;
using UnityContainer = Unity.UnityContainer;

namespace GitHub
{
    [TestClass]
    public class Issues
    {
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
