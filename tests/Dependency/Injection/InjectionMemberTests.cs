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
            var member = GetInjectionMember();

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(TestPolicySet), null, ref cast);

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
            var member = GetInjectionMember();

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(PolicySet), null, ref cast);
        }

        [TestMethod]
        public virtual void BuildRequiredTest()
        {
            // Arrange
            var member = GetInjectionMember();

            // Validate
            Assert.IsTrue(member.BuildRequired);
        }

        [TestMethod]
        public virtual void IsNotInitializedTest()
        {
            // Arrange
            var member = GetInjectionMember();

            // Validate
            Assert.IsFalse(member.IsInitialized);
        }

        [TestMethod]
        public virtual void IsInitializedTest()
        {
            // Arrange
            var member = GetInjectionMember();
            var set = new TestPolicySet();
            var cast = set as IPolicySet;

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(TestPolicySet), null, ref cast);

            // Validate
            Assert.IsTrue(member.IsInitialized);
        }

        #endregion


        #region MemberInfo

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public virtual void MemberInfoNotInitializedTest()
        {
            // Arrange
            var member = GetInjectionMember();

            // Act
            var info = member.MemberInfo(typeof(TestPolicySet));
        }

        [TestMethod]
        public virtual void MemberInfoTest()
        {
            // Arrange
            var set = new TestPolicySet();
            var cast = set as IPolicySet;
            var member = GetInjectionMember();

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(TestPolicySet), null, ref cast);
            var info = member.MemberInfo(typeof(TestPolicySet));

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
            var member = GetInjectionMember();

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(TestPolicySet), null, ref cast);

            // Validate
            Assert.IsNull(member.MemberInfo(typeof(TestClassAttribute)));
        }

        #endregion


        #region Object Overrides

        [TestMethod]
        public virtual void GetHashCodeNotInitialized()
        {
            // Arrange
            var member = GetInjectionMember();

            // Validate
            Assert.AreEqual(0, member.GetHashCode());
        }

        [TestMethod]
        public virtual void GetHashCodeInitialized()
        {
            // Arrange
            var member = GetInjectionMember();
            var set = new TestPolicySet();
            var cast = set as IPolicySet;

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(TestPolicySet), null, ref cast);

            // Validate
            Assert.AreNotEqual(0, member.GetHashCode());
        }

        #endregion


        #region Equitability

        [TestMethod]
        public virtual void EqualsMemberInfoNotInitialized()
        {
            // Arrange
            var member = GetInjectionMember();
            IEquatable<TMemberInfo> equatable = member as IEquatable<TMemberInfo>;
            TMemberInfo info = GetMemberInfo();

            // Validate
            Assert.IsFalse(equatable.Equals(info));
        }

        [TestMethod]
        public virtual void EqualsMemberInfoInitialized()
        {
            // Arrange
            var member = GetInjectionMember();
            var set = new TestPolicySet();
            var cast = set as IPolicySet;
            IEquatable<TMemberInfo> equatable = member;
            TMemberInfo info = GetMemberInfo();

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(TestPolicySet), null, ref cast);

            // Validate
            Assert.IsTrue(equatable.Equals(info));
        }

        [TestMethod]
        public virtual void EqualsObjectNotInitialized()
        {
            // Arrange
            var member = GetInjectionMember();
            object info = GetMemberInfo();

            // Validate
            Assert.IsFalse(member.Equals(info));
        }

        [TestMethod]
        public virtual void EqualsObjectWrong()
        {
            // Arrange
            var member = GetInjectionMember();
            var set = new TestPolicySet();
            var cast = set as IPolicySet;
            object info = GetMemberInfo();

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(TestPolicySet), null, ref cast);

            // Validate
            Assert.IsFalse(member.Equals(this));
        }

        [TestMethod]
        public virtual void EqualsObjectSame()
        {
            // Arrange
            var member = GetInjectionMember();
            var set = new TestPolicySet();
            var cast = set as IPolicySet;
            object info = GetMemberInfo();

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(TestPolicySet), null, ref cast);

            // Validate
            Assert.IsTrue(member.Equals(member));
        }

        [TestMethod]
        public virtual void EqualsObjectInitialized()
        {
            // Arrange
            var member = GetInjectionMember();
            var set = new TestPolicySet();
            var cast = set as IPolicySet;
            object info = GetMemberInfo();

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(TestPolicySet), null, ref cast);

            // Validate
            Assert.IsTrue(member.Equals(info));
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
