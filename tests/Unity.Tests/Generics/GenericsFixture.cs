// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Injection;
using Unity.Lifetime;

namespace Unity.Tests.Generics
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
        /// modified for Lifetime.
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
            container.RegisterType(typeof(IDictionary<,>), typeof(Dictionary<,>), new ExternallyControlledLifetimeManager(), 
                                                                                  new InjectionConstructor());

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
        /// Lifetime passed is null.
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
        /// Passes if Lifetime passed is null. Pass
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
        /// Passes if Lifetime passed is null. Pass
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
                .RegisterType<IRepository<int>, MockRespository<int>>(new ExternallyControlledLifetimeManager());

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
    }
}
