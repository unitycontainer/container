using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reflection;
using Unity.Injection;
using Unity.Policy;
using Unity.Policy.Tests;
using Unity.Resolution;

namespace Injection.Members
{
    [TestClass]
    public class MethodTests : InjectionBaseTests<MethodInfo, object[]>
    {
        [TestMethod]
        public virtual void InfoSelectionTest()
        {
            // Arrange
            var member = new InjectionMethod("TestMethod", "test");
            var set = new PolicySet();
            var cast = set as IPolicySet;

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(MethodTestClass<>), typeof(MethodTestClass<>), null, ref cast);
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
            var set = new PolicySet();
            var cast = set as IPolicySet;

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(MethodTestClass<>), typeof(MethodTestClass<>), null, ref cast);
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
            var set = new PolicySet();
            var cast = set as IPolicySet;

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(MethodTestClass<>), typeof(MethodTestClass<>), null, ref cast);
            var info = member.MemberInfo(typeof(MethodTestClass<>));

            // Validate
            Assert.IsNotNull(info);
            Assert.AreEqual(0, info.GetParameters().ToArray().Length);
        }


        #region Test Data

        protected override InjectionMember<MethodInfo, object[]> GetDefaultMember() => 
            new InjectionMethod("TestMethod");

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
