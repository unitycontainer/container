using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Unity.Injection;

namespace Injection.Matching
{
    [TestClass]
    public class DataMatchTests
    {
        [DataTestMethod]
        [DynamicData(nameof(MatchTestData), DynamicDataSourceType.Method)]
        public void MatchTest(object data, Type match, bool result)
        {
            // Validate
            Assert.AreEqual(result, data.Matches(match));
        }

        [DataTestMethod]
        [DynamicData(nameof(MatchTypeTestTestData), DynamicDataSourceType.Method)]
        public void MatchTypeTest(Type type, Type match, bool result)
        {
            // Validate
            Assert.AreEqual(result, type.MatchesType(match));
        }

        [DataTestMethod]
        [DynamicData(nameof(MatchObjectTestData), DynamicDataSourceType.Method)]
        public void MatchObjectTest(object parameter, Type match, bool result)
        {
            // Validate
            Assert.AreEqual(result, parameter.MatchesObject(match));
        }

        #region Test Data

        public static IEnumerable<object[]> MatchTestData()
        {
            yield return new object[] { null,                                   null,           true };
            yield return new object[] { typeof(object),                         typeof(object), true };
            yield return new object[] { typeof(Type),                           typeof(Type), true };
            yield return new object[] { new ResolvedParameter(typeof(object)),  typeof(object), true };
        }

        public static IEnumerable<object[]> MatchTypeTestTestData()
        {
            yield return new object[] { typeof(Array),      typeof(object[]),   true };
            yield return new object[] { typeof(Array),      typeof(Array),      true };
            yield return new object[] { typeof(object[]),   typeof(Array),      true };
            yield return new object[] { typeof(string[]),   typeof(int[]),      false };
            yield return new object[] { typeof(object),     typeof(object),     true };
            yield return new object[] { typeof(object),     typeof(object[]),   false };
            yield return new object[] { typeof(object[]),   typeof(object),     true };
            yield return new object[] { typeof(object[]),   typeof(object[]),   true };

            yield return new object[] { typeof(string[]),   typeof(string[]),   true };
            yield return new object[] { typeof(string),     typeof(string[]),   false };
            yield return new object[] { typeof(string[]),   typeof(string),     false };

            yield return new object[] { typeof(List<int>),  typeof(IList<int>), true };
            yield return new object[] { typeof(IList<int>), typeof(List<int>),  false };

            yield return new object[] { typeof(List<>),     typeof(IList<>),    false };
            yield return new object[] { typeof(IList<>),    typeof(List<>),     false };

            yield return new object[] { typeof(List<>),     typeof(List<>),     true };
            yield return new object[] { typeof(List<int>),  typeof(List<>),     false };
            yield return new object[] { typeof(List<>),     typeof(List<int>),  true };
            yield return new object[] { typeof(List<int>),  typeof(List<int>),  true };

            yield return new object[] { null,               null,               true };
            yield return new object[] { null,               typeof(object),     true };
        }

        public static IEnumerable<object[]> MatchObjectTestData()
        {
            yield return new object[] { new List<int>[0],  typeof(IList<int>[]), true };
            yield return new object[] { new IList<int>[0], typeof(List<int>[]),  false };

            yield return new object[] { new string[0],     typeof(int[]),        false };
            yield return new object[] { new object[0],     typeof(object[]),     true };
            yield return new object[] { new string[0],     typeof(object[]),     true }; 
            yield return new object[] { new object[0],     typeof(string[]),     false }; 
            yield return new object[] { new string[0],     typeof(object[]),     true };
            yield return new object[] { new string[0],     typeof(string[]),     true };
                                                                                 
            yield return new object[] { new object[0],     typeof(object),       true };
            yield return new object[] { new object[0],     typeof(Array),        true };
            yield return new object[] { new string[0],     typeof(object),       true };
            yield return new object[] { new string[0],     typeof(Array),        true };
            yield return new object[] { new string[0],     typeof(string),       false };
                                                                                 
            yield return new object[] { typeof(object),    typeof(object),       true };
            yield return new object[] { new object(),      typeof(object),       true };
                                                                                 
            yield return new object[] { null,              null,                 true };
            yield return new object[] { null,              typeof(object),       true };
                                                                                 
            yield return new object[] { string.Empty,      typeof(string),       true };
            yield return new object[] { string.Empty,      typeof(object),       true };
            yield return new object[] { string.Empty,      typeof(int),          false };
        }

        #endregion
    }
}
