using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Tests
{
    [TestClass]
    public class PerformanceFixture
    {
        IUnityContainer _container = new UnityContainer(ModeFlags.Compiled);

        [TestInitialize]
        public virtual void SetupContainer()
        {
            _container.RegisterType<PocoWithDependency>();
            _container.RegisterType<IFoo, Foo>();
            _container.RegisterType<IFoo, Foo>("1");
            _container.RegisterFactory<IFoo>("2", c => new Foo());
            _container.RegisterInstance(typeof(Foo), new Foo());
            _container.RegisterType(typeof(IFoo<>), typeof(Foo<>));
        }

        [TestMethod]
        public void UnityContainer()
        {
            // Act
            var instance = _container.Resolve(typeof(IUnityContainer), null) as IUnityContainer;

            // Validate
            Assert.IsNotNull(instance);
        }


        [TestMethod]
        public void Instance()
        {
            // Act
            var instance = _container.Resolve(typeof(Foo), null) as IUnityContainer;

            // Validate
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void UnityContainerAsync()
        {
            // Act
            var instance = _container.Resolve(typeof(IUnityContainerAsync), null) as IUnityContainer;

            // Validate
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void Unregistered()
        {
            // Act
            var instance = _container.Resolve(typeof(Poco), null) as Poco;

            // Validate
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void Transient()
        {
            // Act
            var instance = _container.Resolve(typeof(PocoWithDependency), null) as PocoWithDependency;

            // Validate
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void Mapping()
        {
            // Act
            var instance = _container.Resolve(typeof(IFoo), null) as IFoo;

            // Validate
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void GenericInterface()
        {
            // Act
            var instance = _container.Resolve(typeof(IFoo<IFoo>), null) as IFoo<IFoo>;
        }

        [TestMethod]
        public void Factory()
        {
            // Act
            var instance = _container.Resolve(typeof(IFoo), "2") as IFoo;

            // Validate
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void Array()
        {
            // Act
            var instance = _container.Resolve(typeof(IFoo[]), null) as IFoo[];

            // Validate
            Assert.IsNotNull(instance);
            Assert.AreEqual(2, instance.Length);
        }

        [TestMethod]
        public void Enumerable()
        {
            // Act
            var instance = _container.Resolve(typeof(IEnumerable<IFoo>), null) as IEnumerable<IFoo>;

            // Validate
            Assert.IsNotNull(instance);
            Assert.AreEqual(3, instance.Count());
        }


        #region Test Data
        public class Poco { }

        public class PocoWithDependency
        {
            [Dependency]
            public object Dependency { get; set; }

            [InjectionMethod]
            public object CallMe([Dependency]object data) => data;
        }

        public interface IFoo { }

        public class Foo : IFoo { }

        public interface IFoo<T> { }

        public class Foo<T> : IFoo<T> { }

        #endregion
    }
}
