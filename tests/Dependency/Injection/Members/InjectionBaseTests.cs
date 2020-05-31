using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Injection;
using Unity.Policy;
using Unity.Resolution;
using static Injection.Members.InjectionMemberTests;

namespace Injection.Members
{
    public abstract class InjectionBaseTests<TMemberInfo, TData>
        where TMemberInfo : MemberInfo
    {
        #region Fields

        protected static ConstructorInfo CtorInfo = typeof(PolicySet).GetConstructor(new Type[0]);
        protected static ConstructorInfo CtorStringInfo = typeof(PolicySet).GetConstructor(new Type[] { typeof(string) });
        protected static FieldInfo FieldInfo = typeof(PolicySet).GetField(nameof(PolicySet.NameField));
        protected static PropertyInfo PropertyInfo = typeof(PolicySet).GetProperty(nameof(PolicySet.NameProperty));
        protected static MethodInfo MethodInfo = typeof(PolicySet).GetMethod(nameof(PolicySet.TestMethod));

        #endregion


        public virtual void InitializationTest(InjectionMember<TMemberInfo, TData> member, TMemberInfo info)
        {
            // Arrange
            var set = new PolicySet();
            var cast = set as IPolicySet;

            // Validate
            Assert.IsNotNull(member);
            Assert.IsFalse(member.IsInitialized);

            // Act
            member.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(PolicySet), null, ref cast);

            // Validate
            Assert.IsTrue(member.IsInitialized);
        }


        public virtual void DeclaredMembersTest(InjectionMember<TMemberInfo, TData> member, Type type, TMemberInfo[] infos)
        {
            // Act
            var members = member.DeclaredMembers(type)
                                .ToArray();
            // Validate
            Assert.AreEqual(infos.Length, members.Length);
        }


        #region MemberInfo

        //[DataTestMethod]
        //[DynamicData(nameof(GetAllInjectionMembers), DynamicDataSourceType.Method)]
        //[ExpectedException(typeof(NullReferenceException))]
        //public virtual void MemberInfoNotInitializedTest(InjectionMember member, MemberInfo info)
        //{
        //    // Arrange
        //    MemberInfo selection = null;

        //    // Act
        //    switch (member)
        //    {
        //        case InjectionMember<MemberInfo, object> injector:
        //            selection = injector.MemberInfo(typeof(PolicySet));
        //            break;

        //        case InjectionMember<MemberInfo, object[]> method:
        //            selection = method.MemberInfo(typeof(PolicySet));
        //            break;
        //    }

        //    // Validate
        //    Assert.AreEqual(info, selection);
        //}

        //[DataTestMethod]
        //[DynamicData(nameof(GetAllInjectionMembers), DynamicDataSourceType.Method)]
        //public virtual void MemberInfoTest(InjectionMember member, MemberInfo info)
        //{
        //    // Arrange
        //    var set = new PolicySet();
        //    var cast = set as IPolicySet;
        //    MemberInfo selection = null;

        //    // Act
        //    member.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(PolicySet), null, ref cast);

        //    // Validate
        //    Assert.AreEqual(info, selection);

        //    // Validate
        //    switch (member)
        //    {
        //        case InjectionMember<MemberInfo, object> injector:
        //            Assert.AreEqual(info, injector.MemberInfo(typeof(PolicySet)));
        //            break;

        //        case InjectionMember<MemberInfo, object[]> method:
        //            Assert.AreEqual(info, method.MemberInfo(typeof(PolicySet)));
        //            break;
        //    }
        //}

        //[Ignore]
        //[TestMethod]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public virtual void MemberWrongInfoTest()
        //{
        //    // Arrange
        //    var set = new TestPolicySet();
        //    var cast = set as IPolicySet;
        //    var member = GetInjectionMember();

        //    // Act
        //    member.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(TestPolicySet), null, ref cast);

        //    // Validate
        //    Assert.IsNull(member.MemberInfo(typeof(TestClassAttribute)));
        //}

        #endregion


    }
}
