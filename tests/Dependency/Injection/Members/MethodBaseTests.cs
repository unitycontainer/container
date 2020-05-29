using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;


namespace Injection.Members
{
    [TestClass]
    public abstract class MethodBaseTests<TMemberInfo> : InjectionParameterTests<TMemberInfo, object[]>
        where TMemberInfo : MemberInfo
    {
    }
}
