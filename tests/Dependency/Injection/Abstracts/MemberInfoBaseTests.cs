using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Unity;

namespace Injection.Members
{
    public abstract class MemberInfoBaseTests<TMemberInfo> : MethodBaseTests<TMemberInfo, object>
        where TMemberInfo : MemberInfo
    {
        [TestMethod]
        public virtual void IsRequiredTest()
        {
            var member = GetDefaultMember();
            Assert.IsInstanceOfType(member.Data, typeof(DependencyAttribute));
        }
    }
}
