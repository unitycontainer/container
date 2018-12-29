using Microsoft.Practices.Unity.Tests.TestObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Builder;
using Unity.Extension;
using Unity.Injection;
using Unity.Strategies;
using Unity.Tests.v5.TestObjects;
using Unity.Tests.v5.TestSupport;

namespace Unity.Tests.v5
{
    [TestClass]
    public class UnityContainerFixture
    {
        [TestMethod]
        public void CanCreateObjectFromUnconfiguredContainer()
        {
            var container = new UnityContainer();

            object o = container.Resolve<object>();

            Assert.IsNotNull(o);
        }

        [TestMethod]
        public void CanCreateOptimizingResolver()
        {
            var container = new UnityContainer();

            object o = container.Resolve<object>();
            Thread.Sleep(100);
            Assert.IsNotNull(container.Resolve<object>());
            Thread.Sleep(100);
            Assert.IsNotNull(container.Resolve<object>());
            Thread.Sleep(100);
            Assert.IsNotNull(container.Resolve<object>());
            Thread.Sleep(100);
            Assert.IsNotNull(container.Resolve<object>());
            Thread.Sleep(100);
            Assert.IsNotNull(container.Resolve<object>());
            Thread.Sleep(100);
            Assert.IsNotNull(container.Resolve<object>());
            Thread.Sleep(100);
            Assert.IsNotNull(container.Resolve<object>());
            Thread.Sleep(100);
            Assert.IsNotNull(container.Resolve<object>());
            Thread.Sleep(100);
            Assert.IsNotNull(container.Resolve<object>());
        }

        [TestMethod]
        public void ContainerResolvesRecursiveConstructorDependencies()
        {
            var container = new UnityContainer();
            var dep = container.Resolve<ObjectWithOneDependency>();

            Assert.IsNotNull(dep);
            Assert.IsNotNull(dep.InnerObject);
            Assert.AreNotSame(dep, dep.InnerObject);
        }

        [TestMethod]
        public void ContainerResolvesMultipleRecursiveConstructorDependencies()
        {
            var container = new UnityContainer();
            var dep = container.Resolve<ObjectWithTwoConstructorDependencies>();

            dep.Validate();
        }

        [TestMethod]
        public void CanResolveTypeMapping()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>();

            var logger = container.Resolve<ILogger>();

            Assert.IsNotNull(logger);
            Assert.IsInstanceOfType(logger, typeof(MockLogger));
        }

        [TestMethod]
        public void CanRegisterTypeMappingsWithNames()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>()
                .RegisterType<ILogger, SpecialLogger>("special");

            var defaultLogger = container.Resolve<ILogger>();
            var specialLogger = container.Resolve<ILogger>("special");

            Assert.IsNotNull(defaultLogger);
            Assert.IsNotNull(specialLogger);

            Assert.IsInstanceOfType(defaultLogger, typeof(MockLogger));
            Assert.IsInstanceOfType(specialLogger, typeof(SpecialLogger));
        }

        [TestMethod]
        public void ShouldDoPropertyInjection()
        {
            var container = new UnityContainer();

            var obj = container.Resolve<ObjectWithTwoProperties>();

            obj.Validate();
        }

        [TestMethod]
        public void ShouldDoAllInjections()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>();

            var obj = container.Resolve<ObjectWithLotsOfDependencies>();

            Assert.IsNotNull(obj);
            obj.Validate();
        }

        [TestMethod]
        public void CanGetObjectsUsingNongenericMethod()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ILogger), typeof(MockLogger));

            var logger = container.Resolve(typeof(ILogger));

            Assert.IsNotNull(logger);
            Assert.IsInstanceOfType(logger, typeof(MockLogger));
        }

        [TestMethod]
        public void CanGetNamedObjectsUsingNongenericMethod()
        {
            var container = new UnityContainer()
                .RegisterType(typeof(ILogger), typeof(MockLogger))
                .RegisterType(typeof(ILogger), typeof(SpecialLogger), "special");

            var defaultLogger = container.Resolve(typeof(ILogger)) as ILogger;
            var specialLogger = container.Resolve(typeof(ILogger), "special") as ILogger;

            Assert.IsNotNull(defaultLogger);
            Assert.IsNotNull(specialLogger);

            Assert.IsInstanceOfType(defaultLogger, typeof(MockLogger));
            Assert.IsInstanceOfType(specialLogger, typeof(SpecialLogger));
        }

        [TestMethod]
        public void AllInjectionsWorkFromNongenericMethods()
        {
            var container = new UnityContainer()
                .RegisterType(typeof(ILogger), typeof(MockLogger));

            var obj = (ObjectWithLotsOfDependencies)container.Resolve(typeof(ObjectWithLotsOfDependencies));
            obj.Validate();
        }

        [TestMethod]
        public void ContainerSupportsSingletons()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>(new ContainerControlledLifetimeManager());

            var logger1 = container.Resolve<ILogger>();
            var logger2 = container.Resolve<ILogger>();

            Assert.IsInstanceOfType(logger1, typeof(MockLogger));
            Assert.AreSame(logger1, logger2);
        }

        [TestMethod]
        public void CanCreatedNamedSingletons()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>()
                .RegisterType<ILogger, SpecialLogger>("special", new ContainerControlledLifetimeManager());

            var logger1 = container.Resolve<ILogger>();
            var logger2 = container.Resolve<ILogger>();
            var logger3 = container.Resolve<ILogger>("special");
            var logger4 = container.Resolve<ILogger>("special");

            Assert.AreNotSame(logger1, logger2);
            Assert.AreSame(logger3, logger4);
        }

        [TestMethod]
        public void CanRegisterSingletonsWithNongenericMethods()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>(new ContainerControlledLifetimeManager())
                .RegisterType<ILogger, SpecialLogger>("special", new ContainerControlledLifetimeManager());

            var logger1 = container.Resolve<ILogger>();
            var logger2 = container.Resolve<ILogger>();
            var logger3 = container.Resolve<ILogger>("special");
            var logger4 = container.Resolve<ILogger>("special");

            Assert.AreSame(logger1, logger2);
            Assert.AreSame(logger3, logger4);
            Assert.AreNotSame(logger1, logger3);
        }

        [TestMethod]
        public void DisposingContainerDisposesSingletons()
        {
            var container = new UnityContainer()
                .RegisterType<DisposableObject>(new ContainerControlledLifetimeManager());

            var dobj = container.Resolve<DisposableObject>();

            Assert.IsFalse(dobj.WasDisposed);
            container.Dispose();

            Assert.IsTrue(dobj.WasDisposed);
        }

        [TestMethod]
        public void SingletonsRegisteredAsDefaultGetInjected()
        {
            var container = new UnityContainer()
                .RegisterType<ObjectWithOneDependency>(new ContainerControlledLifetimeManager());

            var dep = container.Resolve<ObjectWithOneDependency>();
            var dep2 = container.Resolve<ObjectWithTwoConstructorDependencies>();

            Assert.AreSame(dep, dep2.OneDep);
        }

        [TestMethod]
        public void CanDoInjectionOnExistingObjects()
        {
            var container = new UnityContainer();

            var o = new ObjectWithTwoProperties();
            
            Assert.IsNull(o.Obj1);
            Assert.IsNull(o.Obj2);

            container.BuildUp(o);

            o.Validate();
        }

        [TestMethod]
        public void CanBuildUpExistingObjectWithNonGenericObject()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>();

            var o = new ObjectUsingLogger();
            var result = container.BuildUp(o);

            Assert.IsInstanceOfType(result, typeof(ObjectUsingLogger));
            Assert.AreSame(o, result);
            Assert.IsNotNull(o.Logger);
            Assert.IsInstanceOfType(o.Logger, typeof(MockLogger));
        }

        [TestMethod]
        public void CanBuildupObjectWithExplicitInterface()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>();

            var o = new ObjectWithExplicitInterface();
            container.BuildUp<ISomeCommonProperties>(o);

            o.ValidateInterface();
        }

        [TestMethod]
        public void CanBuildupObjectWithExplicitInterfaceUsingNongenericMethod()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>();

            var o = new ObjectWithExplicitInterface();
            container.BuildUp(typeof(ISomeCommonProperties), o);

            o.ValidateInterface();

        }

        [TestMethod]
        public void CanUseInstanceAsSingleton()
        {
            var logger = new MockLogger();

            IUnityContainer container = new UnityContainer();
            container.RegisterInstance(typeof(ILogger), "logger", logger, new ContainerControlledLifetimeManager());

            var o = container.Resolve<ILogger>("logger");
            Assert.AreSame(logger, o);
        }


        [TestMethod]
        public void CanUseInstanceAsSingletonViaGenericMethod()
        {
            var logger = new MockLogger();

            var container = new UnityContainer()
                .RegisterInstance<ILogger>("logger", logger);

            var o = container.Resolve<ILogger>("logger");
            Assert.AreSame(logger, o);
        }

        [TestMethod]
        public void DisposingContainerDisposesOwnedInstances()
        {
            var o = new DisposableObject();
            var container = new UnityContainer()
                .RegisterInstance(typeof(object), o);

            container.Dispose();
            Assert.IsTrue(o.WasDisposed);
        }

        [TestMethod]
        public void DisposingContainerDoesNotDisposeUnownedInstances()
        {
            var o = new DisposableObject();
            var container = new UnityContainer()
                .RegisterInstance(typeof(object), o, new ExternallyControlledLifetimeManager());

            container.Dispose();
            Assert.IsFalse(o.WasDisposed);
            GC.KeepAlive(o);
        }

        [TestMethod]
        public void ContainerDefaultsToInstanceOwnership()
        {
            var o = new DisposableObject();
            var container = new UnityContainer()
                .RegisterInstance(typeof(object), o);
            container.Dispose();
            Assert.IsTrue(o.WasDisposed);
        }

        [TestMethod]
        public void ContainerDefaultsToInstanceOwnershipViaGenericMethod()
        {
            var o = new DisposableObject();
            var container = new UnityContainer()
                .RegisterInstance(typeof(DisposableObject), o);
            container.Dispose();
            Assert.IsTrue(o.WasDisposed);
        }

        [TestMethod]
        public void InstanceRegistrationWithoutNameRegistersDefault()
        {
            var l = new MockLogger();
            var container = new UnityContainer()
                .RegisterInstance(typeof(ILogger), l);

            var o = container.Resolve<ILogger>();
            Assert.AreSame(l, o);
        }

        [TestMethod]
        public void InstanceRegistrationWithoutNameRegistersDefaultViaGenericMethod()
        {
            var l = new MockLogger();
            var container = new UnityContainer()
                .RegisterInstance<ILogger>(l);

            var o = container.Resolve<ILogger>();
            Assert.AreSame(l, o);
        }

        [TestMethod]
        public void CanRegisterDefaultInstanceWithoutLifetime()
        {
            var o = new DisposableObject();

            var container = new UnityContainer()
                .RegisterInstance(typeof(object), o, new ExternallyControlledLifetimeManager());

            var result = container.Resolve<object>();
            Assert.IsNotNull(result);
            Assert.AreSame(o, result);

            container.Dispose();
            Assert.IsFalse(o.WasDisposed);
            GC.KeepAlive(o);
        }

        [TestMethod]
        public void CanRegisterDefaultInstanceWithoutLifetimeViaGenericMethod()
        {
            var o = new DisposableObject();

            var container = new UnityContainer()
                .RegisterInstance<object>(o, new ExternallyControlledLifetimeManager());

            object result = container.Resolve<object>();
            Assert.IsNotNull(result);
            Assert.AreSame(o, result);

            container.Dispose();
            Assert.IsFalse(o.WasDisposed);
            GC.KeepAlive(o);
        }

        [TestMethod]
        public void CanSpecifyInjectionConstructorWithDefaultDependencies()
        {
            string sampleString = "Hi there";
            var container = new UnityContainer()
                .RegisterInstance(sampleString);

            var o = container.Resolve<ObjectWithInjectionConstructor>();

            Assert.IsNotNull(o.ConstructorDependency);
            Assert.AreSame(sampleString, o.ConstructorDependency);
        }

        [TestMethod]
        public void CanGetInstancesOfAllRegisteredTypes()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>("mock")
                .RegisterType<ILogger, SpecialLogger>("special")
                .RegisterType<ILogger, MockLogger>("another");

            List<ILogger> loggers = new List<ILogger>(
                container.ResolveAll<ILogger>());

            Assert.AreEqual(3, loggers.Count);
            Assert.IsInstanceOfType(loggers[0], typeof(MockLogger));
            Assert.IsInstanceOfType(loggers[1], typeof(SpecialLogger));
            Assert.IsInstanceOfType(loggers[2], typeof(MockLogger));
        }

        [TestMethod]
        public void GetAllDoesNotReturnTheDefault()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, SpecialLogger>("special")
                .RegisterType<ILogger, MockLogger>();

            var loggers = container.ResolveAll<ILogger>().ToList();

            Assert.AreEqual(1, loggers.Count);
            Assert.IsInstanceOfType(loggers[0], typeof(SpecialLogger));
        }

        [TestMethod]
        public void CanGetAllWithNonGenericMethod()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>("mock")
                .RegisterType<ILogger, SpecialLogger>("special")
                .RegisterType<ILogger, MockLogger>("another");

            var loggers = container.ResolveAll(typeof(ILogger)).ToList();

            Assert.AreEqual(3, loggers.Count);
            Assert.IsInstanceOfType(loggers[0], typeof(MockLogger));
            Assert.IsInstanceOfType(loggers[1], typeof(SpecialLogger));
            Assert.IsInstanceOfType(loggers[2], typeof(MockLogger));
        }

        [TestMethod]
        public void GetAllReturnsRegisteredInstances()
        {
            var l = new MockLogger();

            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>("normal")
                .RegisterType<ILogger, SpecialLogger>("special")
                .RegisterInstance<ILogger>("instance", l);

            var loggers = container.ResolveAll<ILogger>().ToList();

            Assert.AreEqual(3, loggers.Count);
            Assert.IsInstanceOfType(loggers[0], typeof(MockLogger));
            Assert.IsInstanceOfType(loggers[1], typeof(SpecialLogger));
            Assert.AreSame(l, loggers[2]);
        }

        [TestMethod]
        public void CanRegisterLifetimeAsSingleton()
        {
            var container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>()
                .RegisterType<ILogger, SpecialLogger>("special", new ContainerControlledLifetimeManager());

            var logger1 = container.Resolve<ILogger>();
            var logger2 = container.Resolve<ILogger>();
            var logger3 = container.Resolve<ILogger>("special");
            var logger4 = container.Resolve<ILogger>("special");

            Assert.AreNotSame(logger1, logger2);
            Assert.AreSame(logger3, logger4);
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void ShouldThrowIfAttemptsToResolveUnregisteredInterface()
        {
            var container = new UnityContainer();
            container.Resolve<ILogger>();
        }

        [TestMethod]
        public void CanBuildSameTypeTwice()
        {
            var container = new UnityContainer();

            container.Resolve<ObjectWithTwoConstructorDependencies>();
            container.Resolve<ObjectWithTwoConstructorDependencies>();
        }

        [TestMethod]
        public void CanRegisterMultipleStringInstances()
        {
            var container = new UnityContainer();
            string first = "first";
            string second = "second";

            container
                .RegisterInstance<string>(first)
                .RegisterInstance<string>(second);

            var result = container.Resolve<string>();

            Assert.AreEqual(second, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetReasonableExceptionWhenRegisteringNullInstance()
        {
            var container = new UnityContainer();
            container.RegisterInstance<SomeType>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RegisteringTheSameLifetimeManagerTwiceThrows()
        {
            var singleton = new ContainerControlledLifetimeManager();

            new UnityContainer()
                .RegisterType<ILogger, MockLogger>(singleton)
                .RegisterType<ILogger, SpecialLogger>("special", singleton);
        }

        [TestMethod]
        public void CanRegisterGenericTypesAndResolveThem()
        {
            var myDict = new Dictionary<string, string>();
            myDict.Add("One", "two");
            myDict.Add("Two", "three");

            var container = new UnityContainer()
                .RegisterInstance(myDict)
                .RegisterType(typeof(IDictionary<,>), typeof(Dictionary<,>));

            var result = container.Resolve<IDictionary<string, string>>();
            Assert.AreSame(myDict, result);
        }

        [TestMethod]
        public void CanSpecializeGenericsViaTypeMappings()
        {
            var container = new UnityContainer()
                .RegisterType(typeof(IRepository<>), typeof(MockRespository<>))
                .RegisterType<IRepository<SomeType>, SomeTypRepository>();

            var generalResult = container.Resolve<IRepository<string>>();
            var specializedResult = container.Resolve<IRepository<SomeType>>();

            Assert.IsInstanceOfType(generalResult, typeof(MockRespository<string>));
            Assert.IsInstanceOfType(specializedResult, typeof(SomeTypRepository));
        }

        [TestMethod]
        public void ContainerResolvesItself()
        {
            var container = new UnityContainer();

            Assert.AreSame(container, container.Resolve<IUnityContainer>());
        }

        [TestMethod]
        public void ContainerResolvesItselfEvenAfterGarbageCollect()
        {
            var container = new UnityContainer();
            container.AddNewExtension<GarbageCollectingExtension>();

            Assert.IsNotNull(container.Resolve<IUnityContainer>());
        }

        public class GarbageCollectingExtension : UnityContainerExtension
        {
            protected override void Initialize()
            {
                this.Context.Strategies.Add(new GarbageCollectingStrategy(), UnityBuildStage.Setup);
            }

            public class GarbageCollectingStrategy : BuilderStrategy
            {
                public override void PreBuildUp(ref BuilderContext context)
                {
                    GC.Collect();
                }
            }
        }

        [TestMethod]
        public void ChildContainerResolvesChildNotParent()
        {
            IUnityContainer parent = new UnityContainer();
            var child = parent.CreateChildContainer();

            Assert.AreSame(child, child.Resolve<IUnityContainer>());
        }

        [TestMethod]
        public void ParentContainerResolvesParentNotChild()
        {
            IUnityContainer parent = new UnityContainer();
            var child = parent.CreateChildContainer();

            Assert.AreSame(parent, parent.Resolve<IUnityContainer>());

        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void ResolvingOpenGenericGivesInnerInvalidOperationException()
        {
            var container = new UnityContainer()
                .RegisterType(typeof(List<>), new InjectionConstructor(10));

            container.Resolve(typeof(List<>));
        }

        [TestMethod]
        public void ResolvingUnconfiguredPrimitiveDependencyGivesReasonableException()
        {
            ResolvingUnconfiguredPrimitiveGivesResonableException<string>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<bool>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<char>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<float>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<double>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<byte>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<short>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<int>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<long>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<IntPtr>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<UIntPtr>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<ushort>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<uint>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<ulong>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<sbyte>();
        }

        private void ResolvingUnconfiguredPrimitiveGivesResonableException<T>()
        {
            var container = new UnityContainer();
            try
            {
                container.Resolve<TypeWithPrimitiveDependency<T>>();
                Assert.Fail("Expected exception did not occur");
            }
            catch (ResolutionFailedException e)
            {
                return;
            }
        }

        internal class SomeType
        {
        }

        public interface IRepository<TEntity>
        {
        }

        public class MockRespository<TEntity> : IRepository<TEntity>
        {
        }

        public class SomeTypRepository : IRepository<SomeType>
        {
        }

        public class ObjectWithPrivateSetter
        {
            [Dependency]
            public object Obj1 { get; private set; }
        }

        public class TypeWithPrimitiveDependency<T>
        {
            public TypeWithPrimitiveDependency(T dependency)
            {
            }
        }
    }
}
