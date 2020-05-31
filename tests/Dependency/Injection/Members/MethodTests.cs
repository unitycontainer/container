using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Unity.Injection;

namespace Injection.Members
{
    [TestClass]
    public class MethodTests : InjectionBaseTests<MethodInfo, object[]>
    {
        #region Test Data

        protected override InjectionMember<MethodInfo, object[]> GetDefaultMember() => 
            new InjectionMethod(nameof(TestClass.TestMethod));

        #endregion
    }
}
