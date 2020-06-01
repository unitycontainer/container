using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Unity.Injection;

namespace Injection.Matching
{
    [TestClass]
    public class InjectionMatchingTests
    {
        [DataTestMethod]
        [DynamicData(nameof(GetMatchesTypeData), DynamicDataSourceType.Method)]
        public void MatchesType(Type type, Type match, bool result)
        {
            // Validate
            Assert.AreEqual(result, type.MatchesType(match));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetMatchesObjectData), DynamicDataSourceType.Method)]
        public void MatchesObject(object parameter, Type match, bool result)
        {
            // Validate
            Assert.AreEqual(result, parameter.MatchesObject(match));
        }


        #region Test Data

        public static IEnumerable<object[]> GetMatchesTypeData()
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

        public static IEnumerable<object[]> GetMatchesObjectData()
        {
            // Generic

            // Array
   // TODO: yield return new object[] { new string[0],      typeof(int[]),      false };
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
