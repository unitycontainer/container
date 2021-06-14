using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Linq;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Injection;
#endif

namespace Regression
{
    public abstract partial class PatternBase
    {
        #region Array

        protected void Assert_Array_Import(Type type)
        {
            // Act
            var instance = Container.Resolve(type, null) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, type);
            Assert.AreEqual(0, (instance.Value as IList)?.Count ?? -1);

            RegisterArrayTypes();

            // Act
            instance = Container.Resolve(type, null) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, type);
#if BEHAVIOR_V4
            Assert.AreEqual(3, (instance.Value as IList)?.Count ?? -1);
#else
            Assert.AreEqual(4, (instance.Value as IList)?.Count ?? -1);
#endif
        }

        protected void Assert_Array_Import(Type type, InjectionMember injection, object values)
        {
            // Arrange
            Container.RegisterType(null, type, null, null, injection);
            RegisterTypes();

            // Act
            var instance = Container.Resolve(type, null) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, type);

            var list = instance.Value as IList;
            var expected = values as IList;
            
            Assert.IsNotNull(list);
            Assert.IsNotNull(expected);

            for (var i = 0; i < expected.Count; i++)
                Assert.IsTrue(list.Contains(expected[i]));
        }

        protected void Assert_Array_Import(Type definition, Type importType, InjectionMember injection, object[] values)
        {
            // Arrange
            var type = definition.MakeGenericType(importType);

            Container.RegisterType(null, definition, null, null, injection);

            // Act
            var instance = Container.Resolve(type, null) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, type);

            var list = instance.Value as IList;
            Assert.IsNotNull(list);

            for (var i = 0; i < values.Length; i++)
                Assert.IsTrue(list.Contains(values[i]));
        }

        #endregion


        #region Enumerable

        public void Assert_Enumerable_Import(Type type)
        {
            // Act
            var instance = Container.Resolve(type, null) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, type);
            Assert.AreEqual(0, (instance.Value as IEnumerable)?.Cast<object>().Count() ?? -1);

            RegisterArrayTypes();

            // Act
            instance = Container.Resolve(type, null) as PatternBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, type);
            Assert.AreEqual(5, (instance.Value as IEnumerable)?.Cast<object>().Count() ?? -1);
        }
        
        #endregion


        #region Implementation

        private void RegisterArrayTypes()
        {
            Container.RegisterInstance<int>(RegisteredInt)
                     .RegisterInstance<int>("int_0", 0)
                     .RegisterInstance<int>("int_1", 1)
                     .RegisterInstance<int>("int_2", 2)
#if !BEHAVIOR_V4
                     .RegisterInstance<int>("int_3", 3)
#endif

                     .RegisterInstance<string>(RegisteredString)
#if !BEHAVIOR_V4 && !UNITY_V4 // Only Unity v5 and up allow `null` as a value
                     .RegisterInstance<string>("string_0", (string)null)
#endif
                     .RegisterInstance<string>("string_1", "string_1")
                     .RegisterInstance<string>("string_2", "string_2")
                     .RegisterInstance<string>("string_3", "string_3")

                     .RegisterInstance<Unresolvable>(RegisteredUnresolvable)
#if !BEHAVIOR_V4 && !UNITY_V4 // Only Unity v5 and up allow `null` as a value
                     .RegisterInstance<Unresolvable>("Unresolvable_0", (Unresolvable)null)
#endif
                     .RegisterInstance<Unresolvable>("Unresolvable_1", Unresolvable.Create("1"))
                     .RegisterInstance<Unresolvable>("Unresolvable_2", Unresolvable.Create("2"))
                     .RegisterInstance<Unresolvable>("Unresolvable_3", Unresolvable.Create("3"));
        }

        #endregion
    }
}
