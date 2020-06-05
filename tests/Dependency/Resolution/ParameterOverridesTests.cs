using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using Unity.Resolution;

namespace Resolution.Overrides
{
    [TestClass]
    public class ParameterOverridesTests
    {
        private const string Name = "name";
        private static object TestValue = new object();
        private static ResolverOverride Target = new ParameterOverride(typeof(string), Name, TestValue).OnType(typeof(ParameterOverridesTests));
        private ParameterOverrides _parameters;

        [TestInitialize]
        public void TestInitialize()
        {
            _parameters = new ParameterOverrides
            {
                { Name, TestValue },
                { Name, TestValue }
            };
        }

        [TestMethod]
        public void ParametersTest()
        {
            foreach (var parameter in _parameters)
            {
                Assert.IsInstanceOfType(parameter, typeof(ParameterOverride));
            }
        }

        [TestMethod]
        public void ParametersOnTypeTest()
        {
            foreach (var parameter in _parameters.OnType(typeof(ParameterOverridesTests)))
            {
                Assert.IsInstanceOfType(parameter, typeof(ParameterOverride));
                Assert.IsTrue(parameter.Equals(Target));
            }
        }

        [TestMethod]
        public void ParametersOnTypeGeneric()
        {
            foreach (var parameter in _parameters.OnType<ParameterOverridesTests>())
            {
                Assert.IsInstanceOfType(parameter, typeof(ParameterOverride));
                Assert.IsTrue(parameter.Equals(Target));
            }
        }

    }
}
