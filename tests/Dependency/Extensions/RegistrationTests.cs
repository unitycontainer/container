using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity;
using Unity.Injection;

namespace Injection.Extensions
{
    [TestClass]
    public class RegistrationTests
    {
        [DataTestMethod]
        [DynamicData(nameof(GetInjectArrayVariants), DynamicDataSourceType.Method)]
        public void InjectArrayTests(object instance)
        {
            // Validate
            Assert.IsInstanceOfType(instance, typeof(ResolvedArrayParameter));
        }

        public static IEnumerable<object[]> GetInjectArrayVariants()
        {
            yield return new object[] { Inject.Array(typeof(string)) };
            yield return new object[] { Inject.Array(typeof(string), string.Empty) };
            yield return new object[] { Inject.Array<string>(string.Empty) };
        }

        // TODO: Issue 146
        //[DataTestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //[DynamicData(nameof(GetInjectArrayInvalidVariants), DynamicDataSourceType.Method)]
        //public void InjectArrayValidationTests()
        //{
        //    _ = Inject.Array(null);
        //}

        //public static IEnumerable<object[]> GetInjectArrayInvalidVariants()
        //{
        //    // TODO: Issue #146 yield return new object[] { Inject.Array(null) };
        //    yield return new object[] { Inject.Array(typeof(string), string.Empty) };
        //}

        //[DataTestMethod]
        //[DynamicData(nameof(GetInjectParameterVariants), DynamicDataSourceType.Method)]
        //public void InjectParameterTests(object instance)
        //{
        //    // Validate
        //    Assert.IsInstanceOfType(instance, typeof(ParameterBase));
        //}

        //public static IEnumerable<object[]> GetInjectParameterVariants()
        //{
        //    yield return new object[] { Inject.Parameter(new object()) };
        //}

        //[TestMethod]
        //public void InjectFieldTests()
        //{ }

        //[TestMethod]
        //public void InjectPropertyTests()
        //{ }
    }
}
