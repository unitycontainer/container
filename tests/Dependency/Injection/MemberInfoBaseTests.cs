using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;


namespace Injection.Members
{
    [TestClass]
    public abstract class MemberInfoBase<TMemberInfo> : InjectionMemberTests<TMemberInfo, object>
        where TMemberInfo : MemberInfo
    {
    }
}
