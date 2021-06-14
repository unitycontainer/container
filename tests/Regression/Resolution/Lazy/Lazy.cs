using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Injection;
#endif

namespace Resolution
{
    public partial class Lazy
    { 
        [TestMethod]
        public void Registered()
        {
            // Setup
            Service.Instances = 0;

            // Act
            Container.RegisterType<IService, Service>();
            var lazy = Container.Resolve<Lazy<IService>>();

            // Verify
            Assert.AreEqual(0, Service.Instances);
            Assert.IsNotNull(lazy);
            Assert.IsNotNull(lazy.Value);
            Assert.AreEqual(1, Service.Instances);
        }

        [TestMethod]
        public void WithMatchingName()
        {
            // Arrange
            Container.RegisterType(typeof(IList<>), typeof(List<>), new InjectionConstructor());
            Container.RegisterType(typeof(IFoo<>), typeof(Foo<>));
            Container.RegisterType<IService, Service>("1");
            Container.RegisterType<IService, Service>("2");
            Container.RegisterType<IService, OtherService>("3");
            Container.RegisterType<IService, Service>();
            Service.Instances = 0;

            // Act
            var lazy = Container.Resolve<Lazy<IService>>("1");

            // Verify
            Assert.IsNotNull(lazy);
            Assert.IsInstanceOfType(lazy.Value, typeof(Service));
        }

        [TestMethod]
        public void WithOtherName()
        {
            // Arrange
            Container.RegisterType(typeof(IList<>), typeof(List<>), new InjectionConstructor());
            Container.RegisterType(typeof(IFoo<>), typeof(Foo<>));
            Container.RegisterType<IService, Service>("1");
            Container.RegisterType<IService, Service>("2");
            Container.RegisterType<IService, OtherService>("3");
            Container.RegisterType<IService, Service>();
            Service.Instances = 0;

            // Act
            var lazy = Container.Resolve<Lazy<IService>>("3");

            // Verify
            Assert.IsNotNull(lazy);
            Assert.IsInstanceOfType(lazy.Value, typeof(OtherService));
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void WithNotMatchingName()
        {
            // Act
            var lazy = Container.Resolve<Lazy<IService>>("10");

            // Verify
            Assert.IsNotNull(lazy);
            Assert.IsInstanceOfType(lazy.Value, typeof(Service));
        }

        [TestMethod]
        public void WithDependencies()
        {
            // Setup
            Container.RegisterType<IService, EmailService>();
            Container.RegisterType<IBase, Base>();


            // Act
            var lazy = Container.Resolve<Lazy<IBase>>();

            // Verify
            Assert.IsFalse(lazy.IsValueCreated);
            var value = lazy.Value;
            Assert.IsNotNull(value.Service);
        }



        [TestMethod]
        public void WithLazyDependencies()
        {
            // Setup
            Container.RegisterType<Lazy<EmailService>>();
            Container.RegisterType<ILazyDependency, LazyDependency>();

            // Act
            var lazy = Container.Resolve<Lazy<ILazyDependency>>();

            // Verify
            Assert.IsFalse(lazy.IsValueCreated);
            var ld = (LazyDependency)lazy.Value;
            Assert.IsFalse(ld.Service.IsValueCreated);
            Assert.IsNotNull(ld.Service.Value);
        }

        [TestMethod]
        public void RegisterLazyInstanceAndResolve()
        {
            // Setup
            var lazy = new Lazy<EmailService>();
            Container.RegisterInstance(lazy);

            // Act
            var lazy1 = Container.Resolve<Lazy<EmailService>>();
            var lazy3 = Container.Resolve<Lazy<EmailService>>();

            // Verify
            Assert.IsTrue(lazy == lazy1);
            Assert.IsTrue(lazy == lazy3);
            Assert.IsFalse(lazy.IsValueCreated);
        }

        [TestMethod]
        public void BuildupLazyInstance()
        {
            // Setup
            Container.RegisterType<Lazy<EmailService>>();
            var lazyDependency = new Lazy<LazyDependency>();

            // Act
            var lazy = Container.BuildUp(lazyDependency);
            var returned = Container.Resolve<Lazy<LazyDependency>>();

            // Verify
            Assert.AreEqual(lazy.GetType(), lazyDependency.GetType());
            Assert.IsFalse(returned.IsValueCreated);
            var ld = returned.Value;
            Assert.IsFalse(ld.Service.IsValueCreated);
            Assert.IsNotNull(ld.Service.Value);
        }

        [TestMethod]
        public void InjectToNonDefaultConstructorWithLazy()
        {
            // Setup
            Container.RegisterType<Lazy<EmailService>>();
            Container.RegisterType<Lazy<LazyDependency>>();

            // Act
            var resolved = Container.Resolve<Lazy<LazyDependency>>();

            // Verify
            Assert.IsFalse(resolved.IsValueCreated);
            Assert.IsFalse(resolved.Value.Service.IsValueCreated);
            Assert.IsNotNull(resolved.Value.Service.Value);
        }

#if !BEHAVIOR_V4
        [TestMethod]
        public void Enumerable()
        {
            // Setup
            Container.RegisterType<IService, Service>("1");
            Container.RegisterType<IService, Service>("2");
            Container.RegisterType<IService, Service>("3");
            Container.RegisterType<IService, OtherService>();
            Service.Instances = 0;

            // Act
            var lazy = Container.Resolve<Lazy<IEnumerable<IService>>>();

            // Verify
            Assert.AreEqual(0, Service.Instances);
            Assert.IsNotNull(lazy);
            Assert.IsNotNull(lazy.Value);

            var array = lazy.Value.ToArray();
            Assert.IsNotNull(array);
            Assert.AreEqual(3, Service.Instances);
            Assert.AreEqual(4, array.Length);
        }
#endif

        [TestMethod]
        public void Array()
        {
            // Setup
            Container.RegisterType<IService, Service>("1");
            Container.RegisterType<IService, Service>("2");
            Container.RegisterType<IService, Service>("3");
            Container.RegisterType<IService, OtherService>();
            Service.Instances = 0;

            // Act
            var lazy = Container.Resolve<Lazy<IService[]>>();

            // Verify
            Assert.AreEqual(0, Service.Instances);
            Assert.IsNotNull(lazy);
            Assert.IsNotNull(lazy.Value);
            Assert.AreEqual(3, Service.Instances);

            var array = lazy.Value.ToArray();
            Assert.IsNotNull(array);
            Assert.AreEqual(3, array.Length);
        }

    }
}
