using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using Unity.Injection;
using Unity.Policy;
using Unity.Policy.Tests;
using Unity.Resolution;
using Unity;

namespace Injection.Members
{
    [TestClass]
    public class MethodTests : MethodBaseTests<MethodInfo, object[]>
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NameValidationTest()
        {
            _ = new InjectionMethod((string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InfoValidationTest()
        {
            _ = new InjectionMethod((MethodInfo)null);
        }

        [TestMethod]
        public virtual void InfoSelectionTest()
        {
            // Arrange
            var member = new InjectionMethod("TestMethod", "test");

            // Act
            var info = member.MemberInfo(typeof(MethodTestClass<int>));

            // Validate
            Assert.IsNotNull(info);
            Assert.AreEqual(typeof(string), info.GetParameters().First().ParameterType);
        }

        [TestMethod]
        public virtual void OpenGenericSelectionTest()
        {
            // Arrange
            var member = new InjectionMethod("TestMethod", "test");

            // Act
            var info = member.MemberInfo(typeof(MethodTestClass<>));

            // Validate
            Assert.IsNotNull(info);
            Assert.AreEqual(typeof(string), info.GetParameters().First().ParameterType);
        }

        [TestMethod]
        public virtual void NoParametersSelectionTest()
        {
            // Arrange
            var member = new InjectionMethod("OtherTestMethod", (object[])null);

            // Act
            var info = member.MemberInfo(typeof(MethodTestClass<>));

            // Validate
            Assert.IsNotNull(info);
            Assert.AreEqual(0, info.GetParameters().ToArray().Length);
        }


        #region Test Data

        protected override InjectionMember<MethodInfo, object[]> GetDefaultMember() => 
            new InjectionMethod("TestMethod");

        protected override InjectionMember<MethodInfo, object[]> GetMember(Type type, int position, object data)
        {
            var info = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                           .Where(member => !member.IsFamily && !member.IsPrivate && !member.IsStatic)
                           .Take(position)
                           .Last();

            return new InjectionMethod(info, data);
        }

        protected class MethodTestClass<T>
        {
            public void OtherTestMethod() { }
            public void Test_Method(TestClass<T> a) { }
            public void TestMethod(T a) { }
            public void TestMethod(string a) { }
            public void TestMethod(string a, out object b) { b = null; }
        }


        #endregion
    }
}
