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
    }
}
