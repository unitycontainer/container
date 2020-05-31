using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace Injection.Members
{
    [TestClass]
    public class MethodBaseTests : InjectionBasesTests<MemberInfo, object[]>
    { 
    }

    [TestClass]
    public class MemberInfoTests : InjectionBasesTests<MemberInfo, object>
    {
    }

    public abstract class InjectionBasesTests<TMemberInfo, TData>
        where TMemberInfo : MemberInfo
    {
        #region MemberInfo



        //[DataTestMethod]
        //[DynamicData(nameof(GetAllInjectionMembers), DynamicDataSourceType.Method)]
        //public virtual void IsNotInitializedTest(InjectionMember member, MemberInfo _)
        //{
        //    // Validate
        //    if (member is InjectionMember<MemberInfo, object> injector)
        //        Assert.IsFalse(injector.IsInitialized);
        //    else if (member is InjectionMember<MemberInfo, object[]> method)
        //        Assert.IsFalse(method.IsInitialized);
        //    else
        //        Assert.Fail();
        //}

        //[DataTestMethod]
        //[DynamicData(nameof(GetAllInjectionMembers), DynamicDataSourceType.Method)]
        //public virtual void IsInitializedTest(InjectionMember member, MemberInfo _)
        //{
        //    // Arrange
        //    var set = new PolicySet();
        //    var cast = set as IPolicySet;

        //    // Act
        //    member.AddPolicies<IResolveContext, IPolicySet>(typeof(IPolicySet), typeof(PolicySet), null, ref cast);

        //    // Validate
        //    switch (member)
        //    {
        //        case InjectionMember<MemberInfo, object> injector:
        //            Assert.IsTrue(injector.IsInitialized);
        //            break;

        //        case InjectionMember<MemberInfo, object[]> method:
        //            Assert.IsTrue(method.IsInitialized);
        //            break;
        //    }
        //}

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
