using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Unity.Injection;

namespace Injection.Members
{
    [TestClass]
    public partial class InjectionFieldTests : MemberInfoBase<FieldInfo>
    {
        protected override InjectionMember<FieldInfo, object> GetInjectionMember() => new InjectionField(nameof(TestPolicySet.TestField));

        protected override FieldInfo GetMemberInfo() => typeof(TestPolicySet).GetField(nameof(TestPolicySet.TestField));
    }
}
