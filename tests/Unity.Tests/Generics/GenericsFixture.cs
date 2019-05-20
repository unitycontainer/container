using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Injection;
using Unity.Lifetime;

namespace Unity.Tests.v5.Generics
{
    /// <summary>
    /// Summary description for TestGenerics
    /// </summary>
    [TestClass]
    public class GenericsFixture
    {
        public class GenericArrayPropertyDependency<T>
        {
            public T[] Stuff { get; set; }
        }

        /// <summary>
        /// Tries to resolve a generic class that at registration, is open and contains an array property of the generic type.
        /// </summary>
        /// <remarks>See Bug 3849</remarks>
        [TestMethod]
        public void ResolveConfiguredGenericType()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(GenericArrayPropertyDependency<>), "testing",
                    new InjectionProperty("Stuff"))
                .RegisterInstance<string>("first", "first")
                .RegisterInstance<string>("second", "second");

            var result = container.Resolve<GenericArrayPropertyDependency<string>>("testing");

            CollectionAssert.AreEquivalent(new[] { "first", "second" }, result.Stuff);
        }

        /// <summary>
        /// Sample from Unit test cases.
        /// modified for WithLifetime.
        /// pass
        /// </summary>
        [TestMethod]
        public void CanRegisterGenericTypesAndResolveThem()
        {
            IDictionary<string, string> myDict = new Dictionary<string, string>();

            myDict.Add("One", "two");
            myDict.Add("Two", "three");

            IUnityContainer container = new UnityContainer();
            container.RegisterInstance(myDict);
            container.RegisterType(typeof(IDictionary<,>), typeof(Dictionary<,>), new InjectionConstructor());

            IDictionary<string, string> result = container.Resolve<IDictionary<string, string>>();
            Assert.AreSame(myDict, result);
        }

        /// <summary>
        /// Sample from Unit test cases.
        /// </summary>
        [TestMethod]
        public void CanSpecializeGenericsViaTypeMappings()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(IRepository<>), typeof(MockRespository<>))
                .RegisterType<IRepository<Foo>, FooRepository>();

            IRepository<string> generalResult = container.Resolve<IRepository<string>>();
            IRepository<Foo> specializedResult = container.Resolve<IRepository<Foo>>();

            Assert.IsInstanceOfType(generalResult, typeof(MockRespository<string>));
            Assert.IsInstanceOfType(specializedResult, typeof(FooRepository));
        }

        /// <summary>
        /// Using List of int type. Pass
        /// WithLifetime passed is null.
        /// </summary>
        [TestMethod]
        public void Testmethod_NoLifetimeSpecified()
        {
            var myList = new List<int>();
            var container = new UnityContainer()
                .RegisterInstance<IList<int>>(myList)
                .RegisterType<List<int>>();

            var result = container.Resolve<IList<int>>();
            Assert.AreSame(myList, result);
        }

        /// <summary>
        /// check mapping with generics
        /// </summary>
        [TestMethod]
        public void TypeMappingWithExternallyControlled()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterInstance("Test String")
                .RegisterType(typeof(IFoo<>), typeof(Foo<>), new ContainerControlledLifetimeManager());

            IFoo<string> result = container.Resolve<IFoo<string>>();
            Assert.IsInstanceOfType(result, typeof(Foo<string>));
        }

        /// <summary>
        /// Using List of string type.
        /// Passes if WithLifetime passed is null. Pass
        /// </summary>
        [TestMethod]
        public void Testmethod_ListOfString()
        {
            var myList = new List<string>();
            var container = new UnityContainer()
                .RegisterInstance<IList<string>>(myList)
                .RegisterType<List<string>>();

            IList<string> result = container.Resolve<IList<string>>();
            Assert.AreSame(myList, result);
        }

        /// <summary>
        /// Using List of object type.
        /// Passes if WithLifetime passed is null. Pass
        /// </summary>
        [TestMethod]
        public void Testmethod_ListOfObjectType()
        {
            var myList = new List<Foo>();
            var container = new UnityContainer()
                .RegisterInstance(myList)
                .RegisterType<IList<Foo>, List<Foo>>();

            var result = container.Resolve<IList<Foo>>();
            Assert.IsInstanceOfType(result, typeof(List<Foo>));
        }

        /// <summary>
        /// have implemented constructor injection of generic type. Pass
        /// </summary>
        [TestMethod]
        public void Testmethod_ImplementConstructorInjection()
        {
            Refer<int> myRefer = new Refer<int>();
            myRefer.Str = "HiHello";
            IUnityContainer container = new UnityContainer()
                .RegisterInstance<Refer<int>>(myRefer)
                .RegisterType<Refer<int>>();

            Refer<int> result = container.Resolve<Refer<int>>();
            Assert.AreSame(myRefer, myRefer);
        }

        /// <summary>
        /// have implemented constructor injection of generic type. passes
        /// </summary>
        [TestMethod]
        public void Testmethod_ConstrucotorInjectionGenerics()
        {
            Refer<int> myRefer = new Refer<int>();
            myRefer.Str = "HiHello";
            IUnityContainer container = new UnityContainer()
                .RegisterInstance<Refer<int>>(myRefer)
                .RegisterType<IRepository<int>, MockRespository<int>>();

            IRepository<int> result = container.Resolve<IRepository<int>>();
            Assert.IsInstanceOfType(result, typeof(IRepository<int>));
        }

        /// <summary>
        /// Passing a generic class as parameter to List which is generic
        /// </summary>
        [TestMethod]
        public void Testmethod_GenericStack()
        {
            Stack<Foo> obj = new Stack<Foo>();
            IUnityContainer uc = new UnityContainer();
            uc.RegisterInstance<Stack<Foo>>(obj);
            Stack<Foo> obj1 = uc.Resolve<Stack<Foo>>();
            Assert.AreSame(obj1, obj);
        }

        [TestMethod]
        public void Testmethod_CheckPropInjection()
        {
            IUnityContainer container = new UnityContainer()
            .RegisterType<IRepository<int>, MockRespository<int>>();

            IRepository<int> result = container.Resolve<IRepository<int>>();
            Assert.IsNotNull(result);
        }

        public interface IService<T> { }
        public class ServiceA<T> : IService<T> { }
        public class ServiceB<T> : IService<T> { }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FailedResolveAllTest()
        {
            var container = new UnityContainer();

            container.RegisterType<IFoo, Foo>("1");
            container.RegisterFactory<IFoo>("2", c => { throw new System.InvalidOperationException(); });

            container.ResolveAll<IFoo>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FailedResolveEnumerableTest()
        {
            var container = new UnityContainer();

            container.RegisterType<IFoo, Foo>("1");
            container.RegisterFactory<IFoo>("2", c => { throw new System.InvalidOperationException(); });

            var instance = container.Resolve<IEnumerable<IFoo>>().ToArray();

            Assert.Fail("Should never reach this line");
        }

        [TestMethod]
        public void CanResolveOpenGenericCollections()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(IService<>), typeof(ServiceA<>), "A")
                .RegisterType(typeof(IService<>), typeof(ServiceB<>), "B");

            List<IService<int>> result = container.Resolve<IEnumerable<IService<int>>>().ToList();
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(svc => svc is ServiceA<int>));
            Assert.IsTrue(result.Any(svc => svc is ServiceB<int>));
        }

        public class ServiceStruct<T> : IService<T> where T : struct { }

        [TestMethod]
        public void CanResolveStructConstraintsCollections()
        {
            var container = new UnityContainer()
                .RegisterType(typeof(IService<>), typeof(ServiceA<>), "A")
                .RegisterType(typeof(IService<>), typeof(ServiceB<>), "B")
                .RegisterType(typeof(IService<>), typeof(ServiceStruct<>), "Struct");

            var result = container.Resolve<IEnumerable<IService<int>>>().ToList();
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Any(svc => svc is ServiceA<int>));
            Assert.IsTrue(result.Any(svc => svc is ServiceB<int>));
            Assert.IsTrue(result.Any(svc => svc is ServiceStruct<int>));

            List<IService<string>> constrainedResult = container.Resolve<IEnumerable<IService<string>>>().ToList();
            Assert.AreEqual(2, constrainedResult.Count);
            Assert.IsTrue(constrainedResult.Any(svc => svc is ServiceA<string>));
            Assert.IsTrue(constrainedResult.Any(svc => svc is ServiceB<string>));
        }

        public class ServiceClass<T> : IService<T> where T : class { }

        [TestMethod]
        public void CanResolveClassConstraintsCollections()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(IService<>), typeof(ServiceA<>), "A")
                .RegisterType(typeof(IService<>), typeof(ServiceB<>), "B")
                .RegisterType(typeof(IService<>), typeof(ServiceClass<>), "Class");

            List<IService<string>> result = container.Resolve<IEnumerable<IService<string>>>().ToList();
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Any(svc => svc is ServiceA<string>));
            Assert.IsTrue(result.Any(svc => svc is ServiceB<string>));
            Assert.IsTrue(result.Any(svc => svc is ServiceClass<string>));

            List<IService<int>> constrainedResult = container.Resolve<IEnumerable<IService<int>>>().ToList();
            Assert.AreEqual(2, constrainedResult.Count);
            Assert.IsTrue(constrainedResult.Any(svc => svc is ServiceA<int>));
            Assert.IsTrue(constrainedResult.Any(svc => svc is ServiceB<int>));
        }

        public class ServiceNewConstraint<T> : IService<T> where T : new() { }

        public class TypeWithNoPublicNoArgCtors
        {
            public TypeWithNoPublicNoArgCtors(int _) { }
            private TypeWithNoPublicNoArgCtors() { }
        }

        [TestMethod]
        public void CanResolveDefaultCtorConstraintsCollections()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(IService<>), typeof(ServiceA<>), "A")
                .RegisterType(typeof(IService<>), typeof(ServiceB<>), "B")
                .RegisterType(typeof(IService<>), typeof(ServiceNewConstraint<>), "NewConstraint");

            List<IService<int>> result = container.Resolve<IEnumerable<IService<int>>>().ToList();
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Any(svc => svc is ServiceA<int>));
            Assert.IsTrue(result.Any(svc => svc is ServiceB<int>));
            Assert.IsTrue(result.Any(svc => svc is ServiceNewConstraint<int>));

            List<IService<TypeWithNoPublicNoArgCtors>> constrainedResult = container.Resolve<IEnumerable<IService<TypeWithNoPublicNoArgCtors>>>().ToList();
            Assert.AreEqual(2, constrainedResult.Count);
            Assert.IsTrue(constrainedResult.Any(svc => svc is ServiceA<TypeWithNoPublicNoArgCtors>));
            Assert.IsTrue(constrainedResult.Any(svc => svc is ServiceB<TypeWithNoPublicNoArgCtors>));
        }

        public class ServiceInterfaceConstraint<T> : IService<T> where T : IEnumerable { }

        [TestMethod]
        public void CanResolveInterfaceConstraintsCollections()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(IService<>), typeof(ServiceA<>), "A")
                .RegisterType(typeof(IService<>), typeof(ServiceB<>), "B")
                .RegisterType(typeof(IService<>), typeof(ServiceInterfaceConstraint<>), "InterfaceConstraint");

            List<IService<string>> result = container.Resolve<IEnumerable<IService<string>>>().ToList();
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Any(svc => svc is ServiceA<string>));
            Assert.IsTrue(result.Any(svc => svc is ServiceB<string>));
            Assert.IsTrue(result.Any(svc => svc is ServiceInterfaceConstraint<string>));

            List<IService<int>> constrainedResult = container.Resolve<IEnumerable<IService<int>>>().ToList();
            Assert.AreEqual(2, constrainedResult.Count);
            Assert.IsTrue(constrainedResult.Any(svc => svc is ServiceA<int>));
            Assert.IsTrue(constrainedResult.Any(svc => svc is ServiceB<int>));
        }
    }
}
