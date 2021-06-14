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

namespace Registration
{
    public partial class Validation
    {
        public static IEnumerable<object[]> ArgumetTestData
        {
            get
            {
                yield return new object[] { typeof(IService), typeof(Service),  null, null,                                     typeof(TransientLifetimeManager) };
#if !BEHAVIOR_V4
                yield return new object[] { typeof(Service),  typeof(IService), null, null,                                     typeof(TransientLifetimeManager) };
#endif
                yield return new object[] { typeof(object),   typeof(object),   null, null,                                     typeof(TransientLifetimeManager) };
                yield return new object[] { null,             typeof(object),   null, null,                                     typeof(TransientLifetimeManager) };
#if !BEHAVIOR_V4
                yield return new object[] { typeof(object),   null,             null, null,                                     typeof(TransientLifetimeManager) };
#endif
                yield return new object[] { typeof(object),   typeof(object),   Name, null,                                     typeof(TransientLifetimeManager) };
                yield return new object[] { null,             typeof(object),   Name, null,                                     typeof(TransientLifetimeManager) };
#if !BEHAVIOR_V4
                yield return new object[] { typeof(object),   null,             Name, null,                                     typeof(TransientLifetimeManager) };
#endif
                yield return new object[] { typeof(object),   typeof(object),   null, new ContainerControlledLifetimeManager(), typeof(ContainerControlledLifetimeManager) };
                yield return new object[] { null,             typeof(object),   null, new ContainerControlledLifetimeManager(), typeof(ContainerControlledLifetimeManager) };
#if !BEHAVIOR_V4
                yield return new object[] { typeof(object),   null,             null, new ContainerControlledLifetimeManager(), typeof(ContainerControlledLifetimeManager) };
#endif
                yield return new object[] { typeof(object),   typeof(object),   Name, new ContainerControlledLifetimeManager(), typeof(ContainerControlledLifetimeManager) };
                yield return new object[] { null,             typeof(object),   Name, new ContainerControlledLifetimeManager(), typeof(ContainerControlledLifetimeManager) };
#if !BEHAVIOR_V4
                yield return new object[] { typeof(object),   null,             Name, new ContainerControlledLifetimeManager(), typeof(ContainerControlledLifetimeManager) };
#endif
            }
        }

        public static IEnumerable<object[]> ArgumetTestDataFailing
        {
            get
            {
#if BEHAVIOR_V4
                yield return new object[] { typeof(ArgumentException), typeof(Service), typeof(IService), null, new TransientLifetimeManager(), null };
                yield return new object[] { typeof(ArgumentException), typeof(object), null, null, new TransientLifetimeManager(),           null };
                yield return new object[] { typeof(ArgumentException), typeof(object), null, Name, new TransientLifetimeManager(),           null };
                yield return new object[] { typeof(ArgumentException), typeof(object), null, null, new ContainerControlledLifetimeManager(), null };
                yield return new object[] { typeof(ArgumentException), typeof(object), null, Name, new ContainerControlledLifetimeManager(), null };
#endif
                yield return new object[] { typeof(ArgumentException), null,           null, null, null,                                     null };
                yield return new object[] { typeof(ArgumentException), null,           null, Name, null,                                     null };
                yield return new object[] { typeof(ArgumentException), null,           null, null, new ContainerControlledLifetimeManager(), null };
                yield return new object[] { typeof(ArgumentException), null,           null, Name, new ContainerControlledLifetimeManager(), null };
            }
        }


        [DataTestMethod]
        [DynamicData(nameof(ArgumetTestData))]
        public void RegisterType_Validation(Type typeFrom, Type typeTo, string name, LifetimeManager lifetimeManager, Type manager)
        {
            // Act
#if UNITY_V4
            Container.RegisterType(typeFrom, typeTo, name, lifetimeManager);
#else
            Container.RegisterType(typeFrom, typeTo, name, (ITypeLifetimeManager)lifetimeManager);
#endif
            var registeredType = typeFrom ?? typeTo;
            var registration = Container.Registrations.FirstOrDefault(r => r.RegisteredType == registeredType && r.Name == name);

            Assert.IsNotNull(registration);
#if !BEHAVIOR_V4
            Assert.IsInstanceOfType(registration.LifetimeManager, manager);
#endif
        }


        [DataTestMethod]
        [DynamicData(nameof(ArgumetTestDataFailing))]
        public void RegisterType_Failing(Type exception, Type typeFrom, Type typeTo, string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            try
            {
                // Act
#if UNITY_V4
                Container.RegisterType(typeFrom, typeTo, name, lifetimeManager, injectionMembers);
#else
                Container.RegisterType(typeFrom, typeTo, name, (ITypeLifetimeManager)lifetimeManager, injectionMembers);
#endif
                Assert.Fail("Did not throw and exception of type {exception?.Name}");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, exception);
            }
        }
    }
}
