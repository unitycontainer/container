using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Unity.Injection;

namespace Injection.Matching
{
    [TestClass]
    public class ParametersMatchTests
    {
        [DataTestMethod]
        [DynamicData(nameof(GetMatchesData), DynamicDataSourceType.Method)]
        public void MatchesTest(object data, Type match, bool result)
        {
            // Validate
            Assert.AreEqual(result, data.Matches(match));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetTypeMatchesData), DynamicDataSourceType.Method)]
        public void MatchesTypeTest(Type type, Type match, bool result)
        {
            // Validate
            Assert.AreEqual(result, type.MatchesType(match));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetObjectMatchesData), DynamicDataSourceType.Method)]
        public void MatchesObjectTest(object parameter, Type match, bool result)
        {
            // Validate
            Assert.AreEqual(result, parameter.MatchesObject(match));
        }


        #region Test Data

        public static IEnumerable<object[]> GetMatchesData()
        {
            yield return new object[] { null,                                   null,           true };
            yield return new object[] { typeof(object),                         typeof(object), true };
            yield return new object[] { new ResolvedParameter(typeof(object)),  typeof(object), true };
        }

        public static IEnumerable<object[]> GetTypeMatchesData()
        {
            yield return new object[] { null,               null,               true };
            yield return new object[] { null,               typeof(object),     true };

            yield return new object[] { typeof(object),     typeof(object),     true };
            yield return new object[] { typeof(object),     typeof(object[]),   false };
            yield return new object[] { typeof(object[]),   typeof(object),     true };
            yield return new object[] { typeof(object[]),   typeof(object[]),   true };
            yield return new object[] { typeof(object[]),   typeof(Array),      true };
            yield return new object[] { typeof(Array),      typeof(object[]),   true };
            yield return new object[] { typeof(Array),      typeof(Array),      true };

            yield return new object[] { typeof(string[]),   typeof(string[]),   true };
            yield return new object[] { typeof(string),     typeof(string[]),   false };
            yield return new object[] { typeof(string[]),   typeof(string),     false };

            yield return new object[] { typeof(List<>),     typeof(List<>),     true };
            yield return new object[] { typeof(List<int>),  typeof(List<>),     false };
            yield return new object[] { typeof(List<>),     typeof(List<int>),  true };
            yield return new object[] { typeof(List<int>),  typeof(List<int>),  true };
        }

        public static IEnumerable<object[]> GetObjectMatchesData()
        {
            // Generic

            // Array
// TODO: Issue #152 yield return new object[] { new string[0],      typeof(int[]),      false };
            yield return new object[] { new string[0],      typeof(int[]),      true };

            // match.IsAssignableFrom
            yield return new object[] { new object[0],      typeof(object),     true };
            yield return new object[] { new object[0],      typeof(Array),      true };
            yield return new object[] { new object[0],      typeof(object[]),   true };
            yield return new object[] { new object[0],      typeof(string[]),   true };
            yield return new object[] { new string[0],      typeof(object),     true };
            yield return new object[] { new string[0],      typeof(Array),      true };
            yield return new object[] { new string[0],      typeof(object[]),   true };
            yield return new object[] { new string[0],      typeof(string[]),   true };
            yield return new object[] { new string[0],      typeof(string),     false };

            yield return new object[] { typeof(object),     typeof(object),     true };
            yield return new object[] { new object(),       typeof(object),     true };

            yield return new object[] { null,               null,               true };
            yield return new object[] { null,               typeof(object),     true };

            yield return new object[] { string.Empty,       typeof(string),     true };
            yield return new object[] { string.Empty,       typeof(object),     true };
            yield return new object[] { string.Empty,       typeof(int),        false };
        }

        #endregion
    }
}
