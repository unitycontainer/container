using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Unity.Injection;
using Unity.Policy;
using Unity.Policy.Tests;
using Unity.Resolution;

namespace Injection.Members
{
    public partial class InjectionMemberTests
    {
        #region Equals

        [DataTestMethod]
        [DynamicData(nameof(GetNotInitializedMembers), DynamicDataSourceType.Method)]
        public virtual void EqualsCold(InjectionMember member, MemberInfo info)
        {
            // Validate
            Assert.IsFalse(member.Equals(info));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetAllInjectionMembers), DynamicDataSourceType.Method)]
        public virtual void EqualsTest(InjectionMember member, MemberInfo info)
        {
            // Arrange
            var set = new PolicySet();
            var cast = set as IPolicySet;

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(PolicySet), null, ref cast);

            // Validate
            Assert.IsTrue(member.Equals(info));
        }

        #endregion


        #region Object

        [DataTestMethod]
        [DynamicData(nameof(GetNotInitializedMembers), DynamicDataSourceType.Method)]
        public virtual void EqualsObjectCold(InjectionMember member, MemberInfo info)
        {
            // Validate
            Assert.IsFalse(member.Equals(info));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetAllInjectionMembers), DynamicDataSourceType.Method)]
        public virtual void EqualsObjectWrong(InjectionMember member, MemberInfo info)
        {
            // Arrange
            var set = new PolicySet();
            var cast = set as IPolicySet;

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(PolicySet), null, ref cast);

            // Validate
            Assert.IsFalse(member.Equals(this));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetAllInjectionMembers), DynamicDataSourceType.Method)]
        public virtual void EqualsObjectSame(InjectionMember member, MemberInfo info)
        {
            // Arrange
            var set = new PolicySet();
            var cast = set as IPolicySet;

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(PolicySet), null, ref cast);

            // Validate
            Assert.IsTrue(member.Equals(member));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetAllInjectionMembers), DynamicDataSourceType.Method)]
        public virtual void EqualsObjectTest(InjectionMember member, MemberInfo info)
        {
            // Arrange
            var set = new PolicySet();
            var cast = set as IPolicySet;

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(PolicySet), null, ref cast);

            // Validate
            Assert.IsTrue(member.Equals(info));
        }

        #endregion
    }
}
