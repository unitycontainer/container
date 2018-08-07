using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Lifetime;
using Unity.Attributes;
using Unity.Injection;
using Unity.Exceptions;

namespace Unity.Tests.Issues
{
    [TestClass]
    public class GitHubIssues
    {
        public interface IFoo { }

        public class Foo : IFoo
        {
            public Foo(string view) { }
        }

        [TestMethod]
        public void unitycontainer_container_88()
        {
            var ioc = new UnityContainer();
            ioc.RegisterType<IFoo, Foo>(new HierarchicalLifetimeManager());

            var value1 = ioc.CreateChildContainer().Resolve<IFoo>(new Resolution.ParameterOverride("view", "qq").OnType<Foo>());
            var value2 = ioc.CreateChildContainer().Resolve<IFoo>(new Resolution.ParameterOverride("view", "qq").OnType<Foo>());
        }

        [TestMethod]
        public void unitycontainer_container_92()
        {
            var ioc = new UnityContainer();
            ioc.RegisterType<IFoo>(
                string.Empty, 
                new SingletonLifetimeManager(), 
                new InjectionFactory(c => { throw new InvalidOperationException(); }));

            Assert.ThrowsException<ResolutionFailedException>(() => ioc.Resolve<IFoo>());
        }

        [TestMethod]
        public void unitycontainer_unity_204_1()
        {
            var container = new UnityContainer();

            container.RegisterType(typeof(ContextFactory), new PerResolveLifetimeManager());
            container.RegisterType<Service1>();
            container.RegisterType<Service2>();
            container.RegisterType<Repository1>();
            container.RegisterType<Repository2>();

            var service1 = container.Resolve<Service1>();

            Assert.AreEqual(service1.Repository1.Factory.Identity, service1.Repository2.Factory.Identity, "case1");

            var service2 = container.Resolve<Service2>();

            Assert.AreEqual(service2.Service.Repository1.Factory.Identity, service2.Service.Repository2.Factory.Identity, "case2");
        }



        [TestMethod]
        public void unitycontainer_unity_204_2()
        {
            var container = new UnityContainer();
            container.RegisterType(typeof(ContextFactory), new PerResolveLifetimeManager());
            container.RegisterType(typeof(Service1),       new PerResolveLifetimeManager());
            container.RegisterType(typeof(Service2),       new PerResolveLifetimeManager());
            container.RegisterType(typeof(Repository1),    new PerResolveLifetimeManager());
            container.RegisterType(typeof(Repository2),    new PerResolveLifetimeManager());

            var service1 = container.Resolve<Service1>();

            Assert.AreEqual(service1.Repository1.Factory.Identity, service1.Repository2.Factory.Identity, "case1");

            var service2 = container.Resolve<Service2>();

            Assert.AreEqual(service2.Service.Repository1.Factory.Identity, service2.Service.Repository2.Factory.Identity, "case2");
        }



        public class ContextFactory
        {
            public string Identity { get; set; } = Guid.NewGuid().ToString();
        }

        public class Repository1
        {
            public Repository1(ContextFactory factory)
            {
                Factory = factory;
            }

            public ContextFactory Factory { get; }
        }

        public class Repository2
        {
            public Repository2(ContextFactory factory)
            {
                Factory = factory;
            }

            public ContextFactory Factory { get; }
        }

        public class Service1
        {
            public Service1(Repository1 repository1, Repository2 repository2)
            {
                Repository1 = repository1;
                Repository2 = repository2;
            }

            public Repository1 Repository1 { get; }
            public Repository2 Repository2 { get; }
        }

        public class Service2
        {
            public Service2(Service1 service)
            {
                Service = service;
            }

            public Service1 Service { get; }
        }


        [TestMethod]
        public void unitycontainer_container_82()
        {
            // Create root container and register classes in root
            var rootContainer = new UnityContainer();
            rootContainer.RegisterType<MainClass>(new PerResolveLifetimeManager());
            rootContainer.RegisterType<IHostClass, MainClass>();

            // Create a child container
            var childContainer = rootContainer.CreateChildContainer();

            var main2 = childContainer.Resolve<MainClass>();
            Assert.AreEqual(main2, main2.HelperClass.HostClass);
        }

    }

    public class MainClass : IHostClass
    {
        public MainClass()
        {
            Debug.Print("Inside Constructor");
        }

        [Dependency]
        public HelperClass HelperClass { get; set; }

        public void DoSomething()
        {
        }
    }

    public interface IHostClass
    {
        void DoSomething();
    }

    public class HelperClass
    {
        [Dependency]
        public IHostClass HostClass { get; set; }
    }
}
