using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity;
using Unity.Lifetime;

namespace Unity.Tests.v5.CollectionSupport
{
    [TestClass]
    public class EnumerableSupportFixture
    {
        public interface IFoo<out TEntity>
        {
            TEntity Value { get; }
        }

        public class Foo<TEntity> : IFoo<TEntity>
        {
            public Foo()
            {
            }

            public Foo(TEntity value)
            {
                Value = value;
            }

            public TEntity Value { get; }
        }

        public interface IService
        {
        }

        public interface IOtherService
        {
        }

        public class Service : IService, IDisposable
        {
            public string Id { get; } = Guid.NewGuid().ToString();

            public static int Instances;

            public Service()
            {
                Interlocked.Increment(ref Instances);
            }

            public bool Disposed;
            public void Dispose()
            {
                Disposed = true;
            }
        }

        public class OtherService : IService, IOtherService, IDisposable
        {
            [InjectionConstructor]
            public OtherService()
            {

            }

            public OtherService(IUnityContainer container)
            {

            }


            public bool Disposed;
            public void Dispose()
            {
                Disposed = true;
            }
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void ClosedGenericsWinInEnumerable(bool enableDiagnostic)
        {
            // Arrange
            var Name = "name";
            var instance = new Foo<IService>(new OtherService());
            UnityContainer container = new UnityContainer();
            if (enableDiagnostic) container.EnableDiagnostic();
            container.RegisterInstance<IFoo<IService>>(Name, instance)
                .RegisterType(typeof(IFoo<>), typeof(Foo<>), Name)
                .RegisterType<IFoo<IService>, Foo<IService>>("closed")
                .RegisterType<IService, Service>();

            // Act
            var enumerable = container.Resolve<IEnumerable<IFoo<IService>>>();
            var array = enumerable.ToArray();

            // Assert
            Assert.AreEqual(2, array.Length);
            Assert.IsNotNull(array[0]);
            Assert.IsNotNull(array[1]);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void ResolvingAnEnumerableTypeSucceedsIfItWasNotRegistered(bool enableDiagnostic)
        {
            UnityContainer container = new UnityContainer();
            if (enableDiagnostic) container.EnableDiagnostic();
            Assert.IsNotNull(container.Resolve<IEnumerable<TestClass>>());
        }

        [DataTestMethod] 
        [DataRow(true)]
        [DataRow(false)]
        public void ResolvingAnEnumerableWithFactory(bool enableDiagnostic)
        {
            var name = "test";
            var data = new[] { new TestClass(), new TestClass() };

            var container = new UnityContainer();
            if (enableDiagnostic) container.EnableDiagnostic();

            container.RegisterFactory<IEnumerable<TestClass>>(c => data)
                .RegisterFactory<IEnumerable<TestClass>>(name, c => data);

            Assert.AreSame(data, container.Resolve<IEnumerable<TestClass>>());
            Assert.AreSame(data, container.Resolve<IEnumerable<TestClass>>(name));
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void ResolvingAnEnumerableTypeSucceedsIfItWasRegistered(bool enableDiagnostic)
        {
            UnityContainer container = new UnityContainer();
            if (enableDiagnostic) container.EnableDiagnostic();

            IEnumerable<TestClass> enumerable = new TestClass[0];
            container.RegisterInstance(enumerable);

            IEnumerable<TestClass> resolved = container.Resolve<IEnumerable<TestClass>>();

            Assert.AreSame(enumerable, resolved);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void ResolvingAllRegistrationsForaTypeReturnsAnEmptyEnumerableWhenNothingIsRegistered(bool enableDiagnostic)
        {
            UnityContainer container = new UnityContainer();
            if (enableDiagnostic) container.EnableDiagnostic();


            IEnumerable<TestClass> resolved = container.ResolveAll<TestClass>();
            List<TestClass> resolvedList = new List<TestClass>(resolved);

            Assert.AreEqual(0, resolvedList.Count);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void ResolvingAllRegistrationsForaTypeReturnsAnEquivalentEnumerableWhenItemsAreRegistered(bool enableDiagnostic)
        {
            UnityContainer container = new UnityContainer();
            if (enableDiagnostic) container.EnableDiagnostic();

            container.RegisterType<TestClass>("Element1", new ContainerControlledLifetimeManager());
            container.RegisterType<TestClass>("Element2", new ContainerControlledLifetimeManager());
            container.RegisterType<TestClass>("Element3", new ContainerControlledLifetimeManager());

            IEnumerable<TestClass> resolved = container.ResolveAll<TestClass>();
            List<TestClass> resolvedList = new List<TestClass>(resolved);

            Assert.AreEqual(3, resolvedList.Count);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void InjectingAnEnumerableTypeSucceedsIfItWasNotRegistered(bool enableDiagnostic)
        {
            UnityContainer container = new UnityContainer();
            if (enableDiagnostic) container.EnableDiagnostic();

            container.Resolve<TestClassWithEnumerableDependency>();
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void InjectingAnEnumerableTypeSucceedsIfItWasRegistered(bool enableDiagnostic)
        {
            UnityContainer container = new UnityContainer();
            if (enableDiagnostic) container.EnableDiagnostic();

            IEnumerable<TestClass> enumerable = new TestClass[0];
            container.RegisterInstance(enumerable);

            TestClassWithEnumerableDependency resolved = container.Resolve<TestClassWithEnumerableDependency>();

            Assert.AreSame(enumerable, resolved.Dependency);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void InjectingAnEnumerableDependencySucceedsIfNoneWereRegistered(bool enableDiagnostic)
        {
            UnityContainer container = new UnityContainer();
            if (enableDiagnostic) container.EnableDiagnostic();


            TestClassWithDependencyEnumerableProperty resolved = container.Resolve<TestClassWithDependencyEnumerableProperty>();

            Assert.AreEqual(1, resolved.Dependency.Count());
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void InjectingAnInterfaceEnumerableDependencySucceedsIfNoneWereRegistered(bool enableDiagnostic)
        {
            UnityContainer container = new UnityContainer();
            if (enableDiagnostic) container.EnableDiagnostic();


            TestClassWithDependencyInterfaceEnumerableProperty resolved = container.Resolve<TestClassWithDependencyInterfaceEnumerableProperty>();

            Assert.AreEqual(0, resolved.Dependency.Count());
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void InjectingAnEnumerableDependencySucceedsIfSomeWereRegistered(bool enableDiagnostic)
        {
            UnityContainer container = new UnityContainer();
            if (enableDiagnostic) container.EnableDiagnostic();

            container.RegisterType<TestClass>("Element1", new ContainerControlledLifetimeManager());
            container.RegisterType<TestClass>("Element2", new ContainerControlledLifetimeManager());
            container.RegisterType<TestClass>("Element3", new ContainerControlledLifetimeManager());

            TestClassWithDependencyEnumerableProperty resolved = container.Resolve<TestClassWithDependencyEnumerableProperty>();

            Assert.AreEqual(3, resolved.Dependency.Count());
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void ConstructingAnDependencyEnumerableWithNoRegisteredElementsSucceeds(bool enableDiagnostic)
        {
            UnityContainer container = new UnityContainer();
            if (enableDiagnostic) container.EnableDiagnostic();

            TestClassWithDependencyEnumerableConstructor resolved = container.Resolve<TestClassWithDependencyEnumerableConstructor>();

            Assert.AreEqual(1, resolved.Dependency.Count());
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void ConstructingAnDependencyInterfaceEnumerableWithNoRegisteredElementsSucceeds(bool enableDiagnostic)
        {
            UnityContainer container = new UnityContainer();
            if (enableDiagnostic) container.EnableDiagnostic();

            TestClassWithDependencyInterfaceEnumerableConstructor resolved = container.Resolve<TestClassWithDependencyInterfaceEnumerableConstructor>();

            Assert.AreEqual(0, resolved.Dependency.Count());
        }


        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void ConstructingAnDependencyEnumerableWithRegisteredElementsSucceeds(bool enableDiagnostic)
        {
            UnityContainer container = new UnityContainer();
            if (enableDiagnostic) container.EnableDiagnostic();

            container.RegisterType<TestClass>("Element1", new ContainerControlledLifetimeManager());
            container.RegisterType<TestClass>("Element2", new ContainerControlledLifetimeManager());
            container.RegisterType<TestClass>("Element3", new ContainerControlledLifetimeManager());

            TestClassWithDependencyEnumerableConstructor resolved = container.Resolve<TestClassWithDependencyEnumerableConstructor>();

            Assert.AreEqual(3, resolved.Dependency.Count());
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void ConstructingAnDependencyEnumerableTypeSucceedsIfItWasNotRegistered(bool enableDiagnostic)
        {
            UnityContainer container = new UnityContainer();
            if (enableDiagnostic) container.EnableDiagnostic();

            container.Resolve<TestClassWithDependencyEnumerableConstructor>();
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void ConstructingWithMethodInjectionAnDependencyEnumerableWithNoRegisteredElementsSucceeds(bool enableDiagnostic)
        {
            UnityContainer container = new UnityContainer();
            if (enableDiagnostic) container.EnableDiagnostic();

            TestClassWithDependencyEnumerableMethod resolved = container.Resolve<TestClassWithDependencyEnumerableMethod>();

            Assert.AreEqual(1, resolved.Dependency.Count());
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void ConstructingWithMethodInjectionAnDependencyInterfaceEnumerableWithNoRegisteredElementsSucceeds(bool enableDiagnostic)
        {
            IUnityContainer container = new UnityContainer();

            TestClassWithDependencyInterfaceEnumerableMethod resolved = container.Resolve<TestClassWithDependencyInterfaceEnumerableMethod>();

            Assert.AreEqual(0, resolved.Dependency.Count());
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void ConstructingWithMethodInjectionAnDependencyEnumerableWithRegisteredElementsSucceeds(bool enableDiagnostic)
        {
            UnityContainer container = new UnityContainer();
            if (enableDiagnostic) container.EnableDiagnostic();

            container.RegisterType<TestClass>("Element1", new ContainerControlledLifetimeManager());
            container.RegisterType<TestClass>("Element2", new ContainerControlledLifetimeManager());
            container.RegisterType<TestClass>("Element3", new ContainerControlledLifetimeManager());

            TestClassWithDependencyEnumerableMethod resolved = container.Resolve<TestClassWithDependencyEnumerableMethod>();

            Assert.AreEqual(3, resolved.Dependency.Count());
        }
    }

}
