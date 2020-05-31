using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Reflection;
using Unity.Injection;

namespace Injection.Members
{
    [TestClass]
    public class ConstructorTests : InjectionBaseTests<ConstructorInfo, object[]>
    {
        #region Test Data

        protected override InjectionMember<ConstructorInfo, object[]> GetDefaultMember() => 
            new InjectionConstructor();

        #endregion
    }
}
