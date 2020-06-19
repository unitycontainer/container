using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using Unity.Injection;

namespace Injection.Members
{
    [TestClass]
    public class MethodTests : MethodBaseTests<MethodInfo>
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NameValidationTest()
        {
            _ = new InjectionMethod((string)null);
        }


        protected override MethodBase<MethodInfo> GetMatchToMember(string name, object[] data) => new InjectionMethod(name, data);

        protected override MethodInfo[] GetMembers(Type type) => type.GetMethods();
    }
}
