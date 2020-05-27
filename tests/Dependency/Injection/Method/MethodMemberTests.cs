using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Unity.Injection;

namespace Injection.Members
{
    [TestClass]
    public partial class InjectionMethodTests : MethodBaseTests<MethodInfo>
    {
        protected override InjectionMember<MethodInfo, object[]> GetInjectionMember() => new InjectionMethod(nameof(TestPolicySet.TestMethod));

        protected override MethodInfo GetMemberInfo() => typeof(TestPolicySet).GetMethod(nameof(TestPolicySet.TestMethod));
    }
}
