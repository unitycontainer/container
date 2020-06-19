using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using Unity;
using Unity.Injection;
using Unity.Policy.Tests;

namespace Extensions.Tests
{
    [TestClass]
    public class InvokeTests
    {
        #region Fields

        public static ConstructorInfo CtorInfo = typeof(PolicySet).GetConstructor(new Type[0]);

        #endregion


        [DataTestMethod]
        [DynamicData(nameof(InvokeCtorData), DynamicDataSourceType.Method)]
        public void ConstructorTest(object instance)
        {
            Assert.IsInstanceOfType(instance, typeof(InjectionConstructor));
        }

        [TestMethod]
        public void MethodTest()
        {
            Assert.IsInstanceOfType(Invoke.Method(string.Empty),         typeof(InjectionMethod));
            Assert.IsInstanceOfType(Invoke.Method(string.Empty, null),   typeof(InjectionMethod));
            Assert.IsInstanceOfType(Invoke.Method(string.Empty, "null"), typeof(InjectionMethod));
        }


        #region Test Data

        public static IEnumerable<object[]> InvokeCtorData()
        {
            // Ctor()
            yield return new object[] { Invoke.Ctor() };
            yield return new object[] { Invoke.Constructor() };

            // Ctor(params object[] parameters)
            yield return new object[] { Invoke.Ctor((object[])null) };
            yield return new object[] { Invoke.Ctor(new object[] { typeof(Type) }) };
            yield return new object[] { Invoke.Constructor((object[])null) };
            yield return new object[] { Invoke.Constructor(new object[] { typeof(Type) }) };

            // Ctor(params Type[] parameters)
            yield return new object[] { Invoke.Ctor((Type[])null) };
            yield return new object[] { Invoke.Ctor(typeof(Type)) };
            yield return new object[] { Invoke.Constructor((Type[])null) };
            yield return new object[] { Invoke.Constructor(typeof(Type)) };

            // Ctor(ConstructorInfo info, params object[] parameters)
            yield return new object[] { Invoke.Ctor(CtorInfo) };
            yield return new object[] { Invoke.Constructor(CtorInfo) };
        }

        #endregion
    }
}
