using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Injection;
using Unity.Policy;
using Unity.Policy.Tests;
using Unity.Resolution;

namespace Injection.Members
{
    public abstract class InjectionBaseTests<TMemberInfo, TData>
        where TMemberInfo : MemberInfo
    {
        [TestMethod]
        public virtual void InitializationTest()
        {
            // Arrange
            var member = GetDefaultMember();
            var set = new PolicySet();
            var cast = set as IPolicySet;

            // Validate
            Assert.IsNotNull(member);
            Assert.IsFalse(member.IsInitialized);

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(TestClass<>), typeof(TestClass<>), null, ref cast);

            // Validate
            Assert.IsTrue(member.IsInitialized);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public virtual void NoMatchAddPolicies()
        {
            var member = GetDefaultMember();
            var set = new PolicySet();
            var cast = set as IPolicySet;

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(NoMatchClass), typeof(NoMatchClass), null, ref cast);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public virtual void DeclaredMemberCold()
        {
            // Act
            var member = GetDefaultMember();

            // Validate
            _ = member.MemberInfo(typeof(TestClass<object>));
        }

        [TestMethod]
        public virtual void DerivedMemberInfo()
        {
            // Arrange
            var member = GetDefaultMember();
            var set = new PolicySet();
            var cast = set as IPolicySet;

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(TestClass<>), typeof(TestClass<>), null, ref cast);
            TMemberInfo info = member.MemberInfo(typeof(TestClass<int>));

            // Validate
            Assert.AreEqual(member.Name, info.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public virtual void NoMatchMemberInfo()
        {
            // Arrange
            var member = GetDefaultMember();
            var set = new PolicySet();
            var cast = set as IPolicySet;

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(TestClass<>), typeof(TestClass<>), null, ref cast);
            _ = member.MemberInfo(typeof(NoMatchClass));
        }

        [TestMethod]
        public virtual void MemberInfoSimpleTest()
        {
            // Arrange
            var member = GetDefaultMember();
            var set = new PolicySet();
            var cast = set as IPolicySet;

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(SimpleClass), typeof(SimpleClass), null, ref cast);
            var info = member.MemberInfo(typeof(SimpleClass));

            // Validate
            Assert.IsNotNull(info);
        }

        [TestMethod]
        public virtual void MemberInfoTest()
        {
            // Arrange
            var member = GetDefaultMember();
            var set = new PolicySet();
            var cast = set as IPolicySet;

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(TestClass<>), typeof(TestClass<>), null, ref cast);
            var info = member.MemberInfo(typeof(TestClass<object>));

            // Validate
            Assert.IsNotNull(info);
            Assert.AreEqual(typeof(TestClass<object>), info.DeclaringType);
        }

        [TestMethod]
        public virtual void DeclaredMembersTest()
        {
            // Act
            var member = GetDefaultMember();
            var members = member.DeclaredMembers(typeof(TestClass<object>))
                                .ToArray();
            // Validate
            Assert.AreEqual(2, members.Length);
        }

        [TestMethod]
        public void ValidateParametersTest()
        {
            // Arrange
            var member = GetMember(typeof(TestClass<>), 2);
            
            // Act
            var info = member.MemberInfo(typeof(TestClass<object>));

            // Validate
            Assert.IsNotNull(info);
            Assert.AreEqual(typeof(TestClass<object>), info.DeclaringType);
        }


        #region Test Data

        protected abstract InjectionMember<TMemberInfo, TData> GetDefaultMember();

        protected abstract InjectionMember<TMemberInfo, TData> GetMember(Type type, int position, object value = null);

        #endregion
    }

    #region Test Data

    public class SimpleClass
    {
        public string TestField;
        public SimpleClass() => throw new NotImplementedException();
        public string TestProperty { get; set; }
        public void TestMethod(string a) => throw new NotImplementedException();
    }

    public class NoMatchClass
    {
        private NoMatchClass() { }
    }

    public class TestClass<T>
    {
        #region Constructors
        static TestClass() { }
        public TestClass() { }
        private TestClass(string _) { }
        protected TestClass(long _) { }
        internal TestClass(string a, IList<T> b) {}
        #endregion

        #region Fields

#pragma warning disable CS0169
#pragma warning disable CS0649

        public readonly string TestReadonlyField;
        internal string TestInternalField;
        static string TestStaticField;
        public string TestField;
        private string TestPrivateField;
        protected string TestProtectedField;

#pragma warning restore CS0169
#pragma warning restore CS0649

        #endregion

        #region Properties
        internal string TestInternalProperty { get; set; }
        public string TestReadonlyProperty { get; }
        static string TestStaticProperty { get; set; }
        public string TestProperty { get; set; }
        private string TestPrivateProperty { get; set; }
        protected string TestProtectedProperty { get; set; }
        #endregion

        #region Methods
        static void TestMethod() { }
        public void TestMethod(string _) { }
        private void TestMethod(int _) { }
        protected void TestMethod(long _) { }
        public void TestMethod(string a, IList<T> b, out object c) { c = null; }
        #endregion
    }

    #endregion
}
