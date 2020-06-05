using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Unity.Injection;

namespace Injection.Parameters
{
    [TestClass]
    public class ArrayTests
    {
        #region ResolvedArrayParameter

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArrayParameterNullTest()
        {
            new ResolvedArrayParameter(null);
        }

        [DataTestMethod]
        [DynamicData(nameof(ResolvedArrayParameterData), DynamicDataSourceType.Method)]
        public void ArrayParameterCtorTest(Type elementType, object[] elementValues)
        {
            Assert.IsNotNull(new ResolvedArrayParameter(elementType, elementValues));
        }

        [DataTestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [DynamicData(nameof(ResolvedArrayInvalidData), DynamicDataSourceType.Method)]
        public void ArrayParameterInvalid(Type elementType, object[] elementValues)
        {
            Assert.IsNotNull(new ResolvedArrayParameter(elementType, elementValues));
        }

        #endregion


        #region Test Data

        public static IEnumerable<object[]> ResolvedArrayParameterData()
        {
            yield return new object[] { typeof(object),        null };
            yield return new object[] { typeof(object),        new object[] { } };
            yield return new object[] { typeof(object),        new object[] { new InjectionParameter(typeof(object), null) } };
            yield return new object[] { typeof(object),        new object[] { typeof(object) }  };
            yield return new object[] { typeof(IList<string>), new object[] { new List<string>() } };
        }

        public static IEnumerable<object[]> ResolvedArrayInvalidData()
        {
            yield return new object[] { typeof(IList<string>), new object[] { typeof(List<string>) } };
            yield return new object[] { typeof(object), new object[] { null } };
        }

        #endregion
    }
}
