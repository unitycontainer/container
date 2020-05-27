using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Injection;
using Unity.Policy;
using Unity.Resolution;

namespace Injection.Members
{
    [TestClass]
    public abstract class InjectionMemberTests<TMemberInfo, TData> where TMemberInfo : MemberInfo
    {
        #region Initialization

        protected abstract InjectionMember<TMemberInfo, TData> GetInjectionMember();
        
        protected abstract TMemberInfo GetMemberInfo();

        #endregion


        #region InjectionMember

        [TestMethod]
        public virtual void AddPoliciesTest()
        {
            // Arrange
            var set = new TestPolicySet();
            var cast = set as IPolicySet;
            var TestMember = GetInjectionMember();

            // Act
            TestMember.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(TestPolicySet), null, ref cast);

            // Validate
            Assert.AreEqual(0, set.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public virtual void AddWrongTypeTest()
        {
            // Arrange
            var set = new TestPolicySet();
            var cast = set as IPolicySet;
            var TestMember = GetInjectionMember();

            // Act
            TestMember.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(PolicySet), null, ref cast);
        }

        [TestMethod]
        public virtual void BuildRequiredTest()
        {
            // Arrange
            var TestMember = GetInjectionMember();

            // Validate
            Assert.IsTrue(TestMember.BuildRequired);
        }

        [TestMethod]
        public virtual void IsNotInitializedTest()
        {
            // Arrange
            var TestMember = GetInjectionMember();

            // Validate
            Assert.IsFalse(TestMember.IsInitialized);
        }

        [TestMethod]
        public virtual void IsInitializedTest()
        {
            // Arrange
            var TestMember = GetInjectionMember();
            var set = new TestPolicySet();
            var cast = set as IPolicySet;

            // Act
            TestMember.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(TestPolicySet), null, ref cast);

            // Validate
            Assert.IsTrue(TestMember.IsInitialized);
        }

        #endregion


        #region MemberInfo

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public virtual void MemberInfoNotInitializedTest()
        {
            // Arrange
            var TestMember = GetInjectionMember();

            // Act
            var info = TestMember.MemberInfo(typeof(TestPolicySet));
        }

        [TestMethod]
        public virtual void MemberInfoTest()
        {
            // Arrange
            var set = new TestPolicySet();
            var cast = set as IPolicySet;
            var TestMember = GetInjectionMember();

            // Act
            TestMember.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(TestPolicySet), null, ref cast);
            var info = TestMember.MemberInfo(typeof(TestPolicySet));

            // Validate
            Assert.AreEqual(GetMemberInfo(), info);
        }

        [Ignore]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public virtual void MemberWrongInfoTest()
        {
            // Arrange
            var set = new TestPolicySet();
            var cast = set as IPolicySet;
            var TestMember = GetInjectionMember();

            // Act
            TestMember.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(TestPolicySet), null, ref cast);

            // Validate
            Assert.IsNull(TestMember.MemberInfo(typeof(TestClassAttribute)));
        }

        #endregion


        #region Object Overrides

        [TestMethod]
        public virtual void GetHashCodeNotInitialized()
        {
            // Arrange
            var TestMember = GetInjectionMember();

            // Validate
            Assert.AreEqual(0, TestMember.GetHashCode());
        }

        [TestMethod]
        public virtual void GetHashCodeInitialized()
        {
            // Arrange
            var TestMember = GetInjectionMember();
            var set = new TestPolicySet();
            var cast = set as IPolicySet;

            // Act
            TestMember.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(TestPolicySet), null, ref cast);

            // Validate
            Assert.AreNotEqual(0, TestMember.GetHashCode());
        }


        [TestMethod]
        public virtual void ToStringTest()
        {
            // Arrange
            var TestMember = GetInjectionMember();

            // Validate
            Assert.IsFalse(string.IsNullOrWhiteSpace(TestMember.ToString()));
        }

        #endregion


        #region Equitability

        [TestMethod]
        public virtual void EqualsMemberInfoNotInitialized()
        {
            // Arrange
            var TestMember = GetInjectionMember();
            IEquatable<TMemberInfo> equatable = TestMember as IEquatable<TMemberInfo>;
            TMemberInfo info = GetMemberInfo();

            // Validate
            Assert.IsFalse(equatable.Equals(info));
        }

        [TestMethod]
        public virtual void EqualsMemberInfoInitialized()
        {
            // Arrange
            var TestMember = GetInjectionMember();
            var set = new TestPolicySet();
            var cast = set as IPolicySet;
            IEquatable<TMemberInfo> equatable = TestMember;
            TMemberInfo info = GetMemberInfo();

            // Act
            TestMember.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(TestPolicySet), null, ref cast);

            // Validate
            Assert.IsTrue(equatable.Equals(info));
        }

        [TestMethod]
        public virtual void EqualsObjectNotInitialized()
        {
            // Arrange
            var TestMember = GetInjectionMember();
            object info = GetMemberInfo();

            // Validate
            Assert.IsFalse(TestMember.Equals(info));
        }

        [TestMethod]
        public virtual void EqualsObjectWrong()
        {
            // Arrange
            var TestMember = GetInjectionMember();
            var set = new TestPolicySet();
            var cast = set as IPolicySet;
            object info = GetMemberInfo();

            // Act
            TestMember.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(TestPolicySet), null, ref cast);

            // Validate
            Assert.IsFalse(TestMember.Equals(this));
        }

        [TestMethod]
        public virtual void EqualsObjectSame()
        {
            // Arrange
            var TestMember = GetInjectionMember();
            var set = new TestPolicySet();
            var cast = set as IPolicySet;
            object info = GetMemberInfo();

            // Act
            TestMember.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(TestPolicySet), null, ref cast);

            // Validate
            Assert.IsTrue(TestMember.Equals(TestMember));
        }

        [TestMethod]
        public virtual void EqualsObjectInitialized()
        {
            // Arrange
            var TestMember = GetInjectionMember();
            var set = new TestPolicySet();
            var cast = set as IPolicySet;
            object info = GetMemberInfo();

            // Act
            TestMember.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(TestPolicySet), null, ref cast);

            // Validate
            Assert.IsTrue(TestMember.Equals(info));
        }

        #endregion


        #region Test Data

        public class PolicySet : Dictionary<Type, object>, IPolicySet
        {
            private string _name;

            public PolicySet(string name)
            {
                _name = name;
            }

            public void Clear(Type policyInterface)
            {
                Remove(policyInterface);
            }

            public object Get(Type policyInterface)
            {
                return TryGetValue(policyInterface, out object value)
                    ? value : null;
            }

            public void Set(Type policyInterface, object policy)
            {
                Set(policyInterface, policy);
            }
        }

        public class TestPolicySet : PolicySet
        {
            public TestPolicySet()
                : base("test")
            {

            }

            public object TestField;

            public object TestProperty { get; set; }

            public void TestMethod() { }
        }

        #endregion
    }
}
