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
    [TestClass]
    public partial class InjectionMemberTests
    {
        #region Fields

        public static ConstructorInfo CtorInfo = typeof(PolicySet).GetConstructor(new Type[0]);
        public static ConstructorInfo CtorStringInfo = typeof(PolicySet).GetConstructor(new Type[] { typeof(string) });
        public static FieldInfo       FieldInfo = typeof(PolicySet).GetField(nameof(PolicySet.NameField));
        public static PropertyInfo    PropertyInfo = typeof(PolicySet).GetProperty(nameof(PolicySet.NameProperty));
        public static MethodInfo      MethodInfo = typeof(PolicySet).GetMethod(nameof(PolicySet.TestMethod));
        private static MethodInfo     ToStringMethod = typeof(InjectionMember).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                                                                              .Where(i => i.Name == nameof(InjectionMember.ToString))
                                                                              .First();
        #endregion


        #region InjectionMember

        [DataTestMethod]
        [DynamicData(nameof(GetNotInitializedMembers), DynamicDataSourceType.Method)]
        public virtual void AddPoliciesTest(InjectionMember member, MemberInfo _)
        {
            // Arrange
            var set = new PolicySet();
            var cast = set as IPolicySet;

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(PolicySet), null, ref cast);

            // Validate
            Assert.AreEqual(0, set.Count);
        }

        [DataTestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [DynamicData(nameof(GetAllInjectionMembers), DynamicDataSourceType.Method)]
        public virtual void AddWrongTypeTest(InjectionMember member, MemberInfo _)
        {
            // Arrange
            var set = new PolicySet();
            var cast = set as IPolicySet;

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(WrongType), typeof(WrongType), null, ref cast);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetAllInjectionMembers), DynamicDataSourceType.Method)]
        public virtual void BuildRequiredTest(InjectionMember member, MemberInfo _)
        {
            // Validate
            Assert.IsTrue(member.BuildRequired);
        }

        #endregion


        #region Test Data

        public static IEnumerable<object[]> GetNotInitializedMembers()
        {
            yield return new object[] { new InjectionConstructor(), CtorInfo };
            yield return new object[] { new InjectionConstructor(typeof(string)), CtorStringInfo };

            yield return new object[] { new InjectionMethod(nameof(PolicySet.TestMethod)), MethodInfo };
            yield return new object[] { new InjectionMethod(nameof(PolicySet.TestMethod), typeof(Type)), MethodInfo };

            yield return new object[] { new InjectionField(nameof(PolicySet.NameField)), FieldInfo };
            yield return new object[] { new InjectionField(nameof(PolicySet.NameField), string.Empty), FieldInfo };

            yield return new object[] { new InjectionProperty(nameof(PolicySet.NameProperty)), PropertyInfo };
            yield return new object[] { new InjectionProperty(nameof(PolicySet.NameProperty), string.Empty), PropertyInfo };

        }

        public static IEnumerable<object[]> GetAllInjectionMembers()
        {
            yield return new object[] { new InjectionConstructor(), CtorInfo };
            yield return new object[] { new InjectionConstructor(typeof(string)), CtorStringInfo };
            yield return new object[] { new InjectionConstructor(CtorStringInfo, typeof(string)), CtorStringInfo };

            yield return new object[] { new InjectionMethod(nameof(PolicySet.TestMethod)), MethodInfo };
            yield return new object[] { new InjectionMethod(nameof(PolicySet.TestMethod), typeof(Type)), MethodInfo };

            yield return new object[] { new InjectionField(nameof(PolicySet.NameField)), FieldInfo };
            yield return new object[] { new InjectionField(nameof(PolicySet.NameField), string.Empty), FieldInfo };

            yield return new object[] { new InjectionProperty(nameof(PolicySet.NameProperty)), PropertyInfo };
            yield return new object[] { new InjectionProperty(nameof(PolicySet.NameProperty), string.Empty), PropertyInfo };

        }

        public class WrongType
        {
            public WrongType(int a) { }
        }

        #endregion
    }
}
