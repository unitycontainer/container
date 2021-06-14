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
    public partial class Arrays
    {
        public const string Legacy = "legacy";

        [TestMethod, TestProperty(RESOLVING, UNREGISTERED)]
        public void ContainerCanResolveListOfT()
        {
            // Arrange
            Container.RegisterType(typeof(List<>), new InjectionConstructor());

            // Act
            var result = Container.Resolve<List<Service>>();

            // Validate
            Assert.IsNotNull(result);
        }

        [TestMethod, TestProperty(RESOLVING, UNREGISTERED)]
        public void ContainerReturnsEmptyArrayIfNoObjectsRegistered()
        {
            // Act
            var result = Container.Resolve<object[]>();

            // Validate
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod, TestProperty(RESOLVING, UNREGISTERED)]
        public void ResolveReturnsRegisteredObjects()
        {
            // Arrange
            object o1 = new object();
            object o2 = new object();

            Container.RegisterInstance("o1", o1)
                     .RegisterInstance("o2", o2);

            // Act
            var result = Container.Resolve<object[]>();

            // Validate
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.AreSame(o1, result[0]);
            Assert.AreSame(o2, result[1]);
        }

        [TestMethod, TestProperty(RESOLVING, UNREGISTERED)]
        public void ResolveAllReturnsRegisteredObjects()
        {
            // Arrange
            object o1 = new object();
            object o2 = new object();

            Container.RegisterInstance("o1", o1)
                     .RegisterInstance("o2", o2);

            // Act
            var result = Container.ResolveAll<object>()
                                  .ToArray();

            // Validate
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.AreSame(o1, result[0]);
            Assert.AreSame(o2, result[1]);
        }

        [TestMethod, TestProperty(RESOLVING, UNREGISTERED)]
        public void ResolveReturnsRegisteredObjectsForBaseClass()
        {
            // Arrange
            IService o1 = new Service();
            IService o2 = new OtherService();

            Container.RegisterInstance("o1", o1)
                     .RegisterInstance("o2", o2);

            // Act
            var result = Container.Resolve<IService[]>();

            // Validate
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.AreSame(o1, result[0]);
            Assert.AreSame(o2, result[1]);
        }

        [TestMethod, TestProperty(RESOLVING, UNREGISTERED)]
        public void ResolveAllReturnsRegisteredObjectsForBaseClass()
        {
            // Arrange
            IService o1 = new Service();
            IService o2 = new OtherService();

            Container.RegisterInstance("o1", o1)
                     .RegisterInstance("o2", o2);

            // Act
            var result = Container.ResolveAll<IService>()
                                  .ToArray();

            // Validate
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.AreSame(o1, result[0]);
            Assert.AreSame(o2, result[1]);
        }

        [TestMethod, TestProperty(RESOLVING, UNREGISTERED)]
        public void ResolverWithElementsReturnsEmptyArrayIfThereAreNoElements()
        {
            // Arrange
            object o1 = new object();
            object o2 = new object();

            Container.RegisterInstance("o1", o1)
                     .RegisterInstance("o2", o2)

                     .RegisterType<InjectedObject>(new InjectionConstructor(new ResolvedArrayParameter<object>()))

                     .RegisterType<InjectedObject>(Legacy, 
                        new InjectionConstructor(new ResolvedArrayParameter(typeof(object))));

            // Act
            var result = (object[])Container.Resolve<InjectedObject>().InjectedValue;
            var legacy = (object[])Container.Resolve<InjectedObject>(Legacy).InjectedValue;

            // Validate
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);

            Assert.IsNotNull(legacy);
            Assert.AreEqual(0, legacy.Length);
        }

        [TestMethod, TestProperty(RESOLVING, UNREGISTERED)]
        public void ResolverWithElementsReturnsLiteralElements()
        {
            // Arrange
            object o1 = new object();
            object o2 = new object();
            object o3 = new object();

            Container.RegisterInstance("o1", o1)
                     .RegisterInstance("o2", o2)
                     
                     .RegisterType<InjectedObject>(
                        new InjectionConstructor(new InjectionParameter(new object[] { o1, o3 })))

                     .RegisterType<InjectedObject>(Legacy, 
                        new InjectionConstructor(
                            new ResolvedArrayParameter(typeof(object),
                                new InjectionParameter(typeof(object), o1), 
                                new InjectionParameter(typeof(object), o3) )));

            // Act
            var result = (object[])Container.Resolve<InjectedObject>().InjectedValue;
            var legacy  = (object[])Container.Resolve<InjectedObject>().InjectedValue;

            // Validate
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.AreSame(o1, result[0]);
            Assert.AreSame(o3, result[1]);

            Assert.IsNotNull(legacy);
            Assert.AreEqual(2, legacy.Length);
            Assert.AreSame(o1, legacy[0]);
            Assert.AreSame(o3, legacy[1]);
        }

        [TestMethod, TestProperty(RESOLVING, UNREGISTERED)]
        public void ResolverWithElementsReturnsResolvedElements()
        {
            // Arrange
            object o1 = new object();
            object o2 = new object();
            object o3 = new object();

            Container.RegisterInstance("o1", o1)
                     .RegisterInstance("o2", o2)
                     .RegisterInstance("o3", o3)

                     .RegisterType<InjectedObject>(
                        new InjectionConstructor(
                            new ResolvedArrayParameter(typeof(object),
                                new ResolvedParameter<object>("o1"),
                                new ResolvedParameter<object>("o2") )))

                     .RegisterType<InjectedObject>(Legacy,
                        new InjectionConstructor(
                            new ResolvedArrayParameter(typeof(object),
                                new ResolvedParameter<object>("o1"),
                                new ResolvedParameter<object>("o2") )));
            // Act
            var result = (object[])Container.Resolve<InjectedObject>().InjectedValue;
            var legacy = (object[])Container.Resolve<InjectedObject>().InjectedValue;

            // Validate
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.AreSame(o1, result[0]);
            Assert.AreSame(o2, result[1]);

            Assert.IsNotNull(legacy);
            Assert.AreEqual(2, legacy.Length);
            Assert.AreSame(o1, legacy[0]);
            Assert.AreSame(o2, legacy[1]);
        }

        [TestMethod, TestProperty(RESOLVING, UNREGISTERED)]
        public void ResolverWithElementsReturnsResolvedElementsForBaseClass()
        {
            // Arrange
            IService o1 = new Service();
            IService o2 = new OtherService();

            Container.RegisterInstance("o1", o1)
                     .RegisterInstance("o2", o2)
                     .RegisterType<InjectedObject>(new InjectionConstructor(typeof(IService[])))
                     .RegisterType<InjectedObject>(Legacy, new InjectionConstructor(typeof(IService[])));

            // Act
            var result = (IService[])Container.Resolve<InjectedObject>().InjectedValue;
            var legacy = (object[])Container.Resolve<InjectedObject>().InjectedValue;

            // Validate
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.AreSame(o1, result[0]);
            Assert.AreSame(o2, result[1]);

            Assert.IsNotNull(legacy);
            Assert.AreEqual(2, legacy.Length);
            Assert.AreSame(o1, legacy[0]);
            Assert.AreSame(o2, legacy[1]);
        }
    }
}
