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
        [DataTestMethod]
        [DynamicData(nameof(GetNotInitializedMembers), DynamicDataSourceType.Method)]
        public virtual void HashCodeCold(InjectionMember member, MemberInfo _)
        {
            // Act
            var hash = member.GetHashCode();

            // Validate
            Assert.AreEqual(0, hash);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetAllInjectionMembers), DynamicDataSourceType.Method)]
        public virtual void HashCodeTest(InjectionMember member, MemberInfo _)
        {
            // Arrange
            var set = new PolicySet();
            var cast = set as IPolicySet;

            member.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(PolicySet), null, ref cast);

            // Act
            var hash = member.GetHashCode();

            // Validate
            Assert.AreNotEqual(0, hash);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetNotInitializedMembers), DynamicDataSourceType.Method)]
        public virtual void ToStringTest(InjectionMember member, MemberInfo _)
        {
            // Arrange
            var set = new PolicySet();
            var cast = set as IPolicySet;

            // Act
            var debugCold = ToStringMethod.Invoke(member, new object[] { true }) as string;
            var optimized = ToStringMethod.Invoke(member, new object[] { false }) as string;

            // Validate
            Assert.IsFalse(string.IsNullOrWhiteSpace(optimized));
            Assert.IsFalse(string.IsNullOrWhiteSpace(debugCold));
            Assert.IsTrue(debugCold.StartsWith(member.GetType().Name));

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(PolicySet), null, ref cast);
            var debugInitialzed = ToStringMethod.Invoke(member, new object[] { true }) as string;
            var initialized     = ToStringMethod.Invoke(member, new object[] { false }) as string;

            // Validate
            Assert.IsFalse(string.IsNullOrWhiteSpace(debugInitialzed));
            Assert.IsFalse(string.IsNullOrWhiteSpace(initialized    ));

            Assert.AreNotEqual(debugCold, debugInitialzed);
            Assert.AreNotEqual(optimized, initialized);
        }
    }
}
