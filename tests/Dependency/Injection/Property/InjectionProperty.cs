using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Unity.Injection;

namespace Injection.Members
{
    [TestClass]
    public partial class InjectionProperty : MemberInfoBase<PropertyInfo>
    {
        protected override InjectionMember<PropertyInfo, object> GetInjectionMember() => new Unity.Injection.InjectionProperty(nameof(TestPolicySet.TestProperty));

        protected override PropertyInfo GetMemberInfo() => typeof(TestPolicySet).GetProperty(nameof(TestPolicySet.TestProperty));
    }
}
