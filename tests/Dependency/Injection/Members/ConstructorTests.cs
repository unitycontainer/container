using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Reflection;
using Unity.Injection;

namespace Injection.Members
{
    [TestClass]
    public class ConstructorTests : InjectionBaseTests<ConstructorInfo, object[]>
    {
        [DataTestMethod]
        [DynamicData(nameof(GetNotInitializedConstructorss), DynamicDataSourceType.Method)]
        public void CtorInitializationTest(InjectionConstructor member, ConstructorInfo _) => base.InitializationTest(member, _);


        #region Test data

        public static IEnumerable<object[]> GetNotInitializedConstructorss()
        {
            yield return new object[] { new InjectionConstructor(),                               CtorInfo       };
            yield return new object[] { new InjectionConstructor(typeof(string)),                 CtorStringInfo };
        }

        public static IEnumerable<object[]> GetInjectionConstructorss()
        {
            yield return new object[] { new InjectionConstructor(),                               CtorInfo       };
            yield return new object[] { new InjectionConstructor(typeof(string)),                 CtorStringInfo };
            yield return new object[] { new InjectionConstructor(CtorStringInfo, typeof(string)), CtorStringInfo };
        }

        #endregion
    }
}
