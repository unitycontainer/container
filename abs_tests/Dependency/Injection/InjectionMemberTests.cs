using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Unity.Injection;

namespace Injection.Members
{
    [TestClass]
    public abstract class InjectionMemberTests<TMemberInfo, TData>
        where TMemberInfo : MemberInfo
        where TData : class
    {

        public abstract InjectionMember<TMemberInfo, TData> GetInjectionMember();

        [TestMethod]
        public void BuildRequiredTest() => Assert.IsTrue(GetInjectionMember().BuildRequired);
    }
}
