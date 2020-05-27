using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using Unity.Injection;

namespace Injection.Members
{
    [TestClass]
    public partial class InjectionConstructorTests : MethodBaseTests<ConstructorInfo>
    {
        protected override InjectionMember<ConstructorInfo, object[]> GetInjectionMember() => new InjectionConstructor();

        protected override ConstructorInfo GetMemberInfo() => typeof(TestPolicySet).GetConstructor(new Type[0]);
    }
}
