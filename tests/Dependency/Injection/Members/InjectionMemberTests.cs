using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Injection;
using Unity.Policy;
using Unity.Resolution;

namespace Injection.Members
{
    [TestClass]
    public class InjectionMemberTests
    {
        #region Fields

        private static ConstructorInfo CtorInfo       = typeof(PolicySet).GetConstructor(new Type[0]);
        private static ConstructorInfo CtorStringInfo = typeof(PolicySet).GetConstructor(new Type[] { typeof(string) });
        private static FieldInfo       FieldInfo      = typeof(PolicySet).GetField(nameof(PolicySet.NameField));
        private static PropertyInfo    PropertyInfo   = typeof(PolicySet).GetProperty(nameof(PolicySet.NameProperty));
        private static MethodInfo      MethodInfo     = typeof(PolicySet).GetMethod(nameof(PolicySet.TestMethod));
        private static MethodInfo      ToStringMethod = typeof(InjectionMember).GetMethods(BindingFlags.Instance|BindingFlags.NonPublic)
                                                                               .Where(i => i.Name == nameof(InjectionMember.ToString))
                                                                               .First();
        #endregion


        #region Initialization


        #endregion


        #region InjectionMember

        [DataTestMethod]
        [DynamicData(nameof(GetNotInitializedMembers), DynamicDataSourceType.Method)]
        public virtual void NoAddedPoliciesTest(InjectionMember member, MemberInfo _)
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


        #region Object Overrides

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
        [DynamicData(nameof(GetAllInjectionMembers), DynamicDataSourceType.Method)]
        public virtual void ToStringTest(InjectionMember member, MemberInfo _)
        {
            // Act
            var debug     = ToStringMethod.Invoke(member, new object[] { true }) as string;
            var optimized = ToStringMethod.Invoke(member, new object[] { false }) as string;

            // Validate
            Assert.IsFalse(string.IsNullOrWhiteSpace(optimized));
            Assert.IsFalse(string.IsNullOrWhiteSpace(debug));
            Assert.IsTrue(debug.StartsWith(member.GetType().Name));
        }

        #endregion


        #region Equitability

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
            yield return new object[] { new InjectionConstructor()                                  , CtorInfo };
            yield return new object[] { new InjectionConstructor(typeof(string))                    , CtorStringInfo };
            yield return new object[] { new InjectionConstructor(CtorStringInfo, typeof(string))    , CtorStringInfo };

            yield return new object[] { new InjectionMethod(nameof(PolicySet.TestMethod))                , MethodInfo };
            yield return new object[] { new InjectionMethod(nameof(PolicySet.TestMethod), typeof(Type))  , MethodInfo };

            yield return new object[] { new InjectionField(nameof(PolicySet.NameField))               , FieldInfo };
            yield return new object[] { new InjectionField(nameof(PolicySet.NameField), string.Empty) , FieldInfo }; 
                                                                                                                 
            yield return new object[] { new InjectionProperty(nameof(PolicySet.NameProperty))               , PropertyInfo };
            yield return new object[] { new InjectionProperty(nameof(PolicySet.NameProperty), string.Empty) , PropertyInfo };

        }

        public class PolicySet : Dictionary<Type, object>, IPolicySet
        {
            public string NameField;
            public string NameProperty { get; set; }

            public PolicySet()
            {
            }

            public PolicySet(string name)
            {
                NameField = name;
                NameProperty = name;
            }

            public void TestMethod(Type @interface) => throw new NotImplementedException();

            public void Clear(Type policyInterface) => Remove(policyInterface);

            public object Get(Type policyInterface) => 
                TryGetValue(policyInterface, out object value)
                    ? value : null;

            public void Set(Type policyInterface, object policy) => Set(policyInterface, policy);
        }

        public class WrongType
        {
            public WrongType(int a)
            {

            }
        }

        #endregion
    }
}
