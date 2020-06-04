using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using Unity.Injection;

namespace Injection.Members
{
    [TestClass]
    public class ConstructorTests : InjectionBaseTests<ConstructorInfo, object[]>
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InfoNullTest()
        {
            _ = new InjectionConstructor((ConstructorInfo)null);
        }

        #region Test Data

        protected override InjectionMember<ConstructorInfo, object[]> GetDefaultMember() => 
            new InjectionConstructor();

        #endregion
    }
}
