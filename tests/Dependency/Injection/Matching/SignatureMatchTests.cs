using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Injection;

namespace Injection.Matching
{
    [TestClass]
    public class SignatureMatchTests
    {
        [DataTestMethod]
        [DynamicData(nameof(GetSignatureMatchData), DynamicDataSourceType.Method)]
        public void SignatureMatchTest(object[] data, MethodBase info, bool result)
        {
            // Validate
            Assert.AreEqual(result, data.MatchMemberInfo(info));
        }

        #region Test Data

        public static IEnumerable<object[]> GetSignatureMatchData()
        {
            var type = typeof(TestClass);

            yield return new object[] { null,                           type.GetMethod(nameof(TestClass.Test1)), true };
            yield return new object[] { null,                           type.GetMethod(nameof(TestClass.Test2)), false };
            yield return new object[] { new object[0],                  type.GetMethod(nameof(TestClass.Test1)), true  };
            yield return new object[] { new object[] { typeof(Type) },  type.GetMethod(nameof(TestClass.Test1)), false };
            yield return new object[] { new object[0],                  type.GetMethod(nameof(TestClass.Test2)), false };
            yield return new object[] { new object[] { typeof(int) },   type.GetMethod(nameof(TestClass.Test2)), true };
            yield return new object[] { new object[] { typeof(Type) },  type.GetMethod(nameof(TestClass.Test2)), false };
        }

        public class TestClass
        {
            public void Test1() { }
            public void Test2(int i) { }
        }

        #endregion
    }
}
