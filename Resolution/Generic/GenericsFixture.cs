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
using Unity.Lifetime;
#endif

namespace Resolution
{
    public partial class Generics
    {

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void WhenResolvingAnOpenGenericType()
        {
            Container.Resolve(typeof(List<>));
        }

        /// <summary>
        /// Sample from Unit test cases.
        /// modified for WithLifetime.
        /// pass
        /// </summary>
        [TestMethod]
        public void CanRegisterGenericTypesAndResolveThem()
        {
            // Arrange
            IDictionary<string, string> myDict = new Dictionary<string, string>();

            myDict.Add("One", "two");
            myDict.Add("Two", "three");

            Container.RegisterInstance(myDict);
            Container.RegisterType(typeof(IDictionary<,>), typeof(Dictionary<,>), new InjectionConstructor());

            // Act
            IDictionary<string, string> result = Container.Resolve<IDictionary<string, string>>();

            // Validate
            Assert.AreSame(myDict, result);
        }

        /// <summary>
        /// Sample from Unit test cases.
        /// </summary>
        [TestMethod]
        public void CanSpecializeGenericsViaTypeMappings()
        {
            // Arrange
            Container.RegisterInstance(Name)
                     .RegisterType(typeof(IFoo<>), typeof(Foo<>))
                     .RegisterType<IFoo<Service>, ServiceFoo>();

            // Act
            IFoo<string> generalResult = Container.Resolve<IFoo<string>>();
            IFoo<Service> specializedResult = Container.Resolve<IFoo<Service>>();

            // Validate
            Assert.IsInstanceOfType(generalResult, typeof(Foo<string>));
            Assert.IsInstanceOfType(specializedResult, typeof(ServiceFoo));
        }

        /// <summary>
        /// Using List of int type. Pass
        /// WithLifetime passed is null.
        /// </summary>
        [TestMethod]
        public void NoLifetimeSpecified()
        {
            // Arrange
            var myList = new List<int>();

            Container.RegisterInstance<IList<int>>(myList)
                     .RegisterType<List<int>>();

            // Act
            var result = Container.Resolve<IList<int>>();

            // Validate
            Assert.AreSame(myList, result);
        }

        /// <summary>
        /// check mapping with generics
        /// </summary>
        [TestMethod]
        public void TypeMappingWithExternallyControlled()
        {
            // Arrange
            Container.RegisterInstance("Test String")
                     .RegisterType(typeof(IFoo<>), typeof(Foo<>), new ContainerControlledLifetimeManager());

            // Act
            IFoo<string> result = Container.Resolve<IFoo<string>>();

            // Validate
            Assert.IsInstanceOfType(result, typeof(Foo<string>));
        }

        /// <summary>
        /// Using List of string type.
        /// Passes if WithLifetime passed is null. Pass
        /// </summary>
        [TestMethod]
        public void ListOfString()
        {
            // Arrange
            var myList = new List<string>();
            Container.RegisterInstance<IList<string>>(myList)
                     .RegisterType<List<string>>();

            // Act
            IList<string> result = Container.Resolve<IList<string>>();

            // Validate
            Assert.AreSame(myList, result);
        }

        /// <summary>
        /// Using List of object type.
        /// Passes if WithLifetime passed is null. Pass
        /// </summary>
        [TestMethod]
        public void ListOfObjectType()
        {
            // Arrange
            var myList = new List<Service>();

            Container.RegisterInstance(myList)
                     .RegisterType<IList<Service>, List<Service>>();

            // Act
            var result = Container.Resolve<IList<Service>>();

            // Validate
            Assert.IsInstanceOfType(result, typeof(List<Service>));
        }

        /// <summary>
        /// have implemented constructor injection of generic type. Pass
        /// </summary>
        [TestMethod]
        public void ImplementConstructorInjection()
        {
            // Arrange
            Refer<int> myRefer = new Refer<int>();
            myRefer.Str = "HiHello";
            Container.RegisterInstance<Refer<int>>(myRefer)
                     .RegisterType<Refer<int>>();

            // Act
            Refer<int> result = Container.Resolve<Refer<int>>();

            // Validate
            Assert.AreSame(myRefer, myRefer);
        }

        /// <summary>
        /// Passing a generic class as parameter to List which is generic
        /// </summary>
        [TestMethod]
        public void GenericStack()
        {
            // Arrange
            Stack<Service> obj = new Stack<Service>();
            Container.RegisterInstance<Stack<Service>>(obj);

            // Act
            Stack<Service> obj1 = Container.Resolve<Stack<Service>>();

            // Validate
            Assert.AreSame(obj1, obj);
        }

        [TestMethod]
#if UNITY_V4 // Unity v4 wraps all exceptions
        [ExpectedException(typeof(ResolutionFailedException))]
#else
        [ExpectedException(typeof(InvalidOperationException))]
#endif
        public void UserExceptionIsNotWrappadWhenResolutionFailed()
        {
            // Arrange
            Container.RegisterType<IService, Service>("1");
#if UNITY_V4
            Container.RegisterType<IService>("2", new InjectionFactory(c => { throw new InvalidOperationException(); }));
#else
            Container.RegisterFactory<IService>("2", c => { throw new InvalidOperationException(); });
#endif
            // Act
            var array = Container.Resolve<IEnumerable<IService>>().ToArray();

            Assert.Fail("Should throw at the line above");
        }

#if !BEHAVIOR_V4
        [TestMethod]
        public void CanResolveOpenGenericCollections()
        {
            // Arrange
            Container.RegisterType(typeof(IGenericService<>), typeof(ServiceA<>), "A")
                     .RegisterType(typeof(IGenericService<>), typeof(ServiceB<>), "B");

            // Act
            List<IGenericService<int>> result = Container.Resolve<IEnumerable<IGenericService<int>>>().ToList();

            // Validate
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(svc => svc is ServiceA<int>));
            Assert.IsTrue(result.Any(svc => svc is ServiceB<int>));
        }
#endif
    }
}
