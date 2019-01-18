using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.Tests.v5.Issues
{
    [TestClass]
    public class GitHubIssues
    {
        public interface IFoo
        {
            string View { get; }
        }

        public class Foo : IFoo
        {
            public string View { get; }

            public Foo(string view)
            {
                View = view;
            }
        }

        //[TestMethod]
        //public void unitycontainer_container_108_without_any_registrations()
        //{
        //    var ioc = new UnityContainer();
        //    var child = ioc.CreateChildContainer();
        //    var spyStrategy = new TestDoubles.SpyStrategy();
        //    child.AddExtension(new TestDoubles.SpyExtension(spyStrategy, UnityBuildStage.PreCreation));

        //    child.Resolve<EmailService>();

        //    Assert.AreEqual(typeof(EmailService), spyStrategy.BuildKey.Type);
        //}

        //[TestMethod]
        //public void unitycontainer_container_108_with_some_existing_registration()
        //{
        //    var ioc = new UnityContainer();
        //    var child = ioc.CreateChildContainer();
        //    var spyStrategy = new TestDoubles.SpyStrategy();
        //    child.AddExtension(new TestDoubles.SpyExtension(spyStrategy, UnityBuildStage.PreCreation));

        //    child.RegisterType<IService, EmailService>();
        //    child.Resolve<IService>();
        //    child.Resolve<EmailService>();

        //    Assert.AreEqual(typeof(EmailService), spyStrategy.BuildKey.Type);
        //}

        //[TestMethod]
        //public void unitycontainer_container_108_with_some_preexisting_registration()
        //{
        //    var ioc = new UnityContainer();
        //    var child = ioc.CreateChildContainer();
        //    var spyStrategy = new TestDoubles.SpyStrategy();
        //    child.RegisterType<IService, EmailService>();

        //    child.AddExtension(new TestDoubles.SpyExtension(spyStrategy, UnityBuildStage.PreCreation));

        //    child.Resolve<IService>();
        //    child.Resolve<EmailService>();

        //    Assert.AreEqual(typeof(EmailService), spyStrategy.BuildKey.Type);
        //}

        //[TestMethod]
        //public void unitycontainer_container_108_for_dependencies()
        //{
        //    var child = new UnityContainer().CreateChildContainer();
        //    var spyStrategy = new TestDoubles.SpyStrategy();
        //    child.AddExtension(new TestDoubles.SpyExtension(spyStrategy, UnityBuildStage.PreCreation));

        //    child.RegisterType<ObjectWithOneDependency, ObjectWithOneDependency>();
        //    child.Resolve<ObjectWithOneDependency>();

        //    var innerDependencyType = typeof(object);
        //    Assert.AreEqual(innerDependencyType, spyStrategy.BuildKey.Type);
        //}

        [TestMethod]
        public void unitycontainer_container_88()
        {
            var str1 = "s1";
            var str2 = "s2";

            IUnityContainer ioc = new UnityContainer();
            ioc.RegisterType<IFoo, Foo>(new HierarchicalLifetimeManager());

            var ch1 = ioc.CreateChildContainer();
            var ch2 = ioc.CreateChildContainer();

            var value1 = ch1.Resolve<IFoo>(new ParameterOverride("view", str1).OnType<Foo>());
            var value2 = ch2.Resolve<IFoo>(new ParameterOverride("view", str2).OnType<Foo>());

            Assert.IsNotNull(value1);
            Assert.IsNotNull(value2);

            Assert.AreEqual(value1.View, str1);
            Assert.AreEqual(value2.View, str2);
        }

        [TestMethod]
        public void unitycontainer_container_92()
        {
            var ioc = new UnityContainer();
            ioc.RegisterFactory<IFoo>(
                string.Empty, 
                c => { throw new InvalidOperationException(); },
                new SingletonLifetimeManager());

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
            IUnityContainer rootContainer = new UnityContainer();
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
