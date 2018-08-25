using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Builder;
using Unity.Policy.Mapping;

namespace Unity.Tests.v5.ObjectBuilder
{
    [TestClass]
    public class BuildKeyMappingPolicyTest
    {
        [TestMethod]
        public void PolicyReturnsNewBuildKey()
        {
            var policy = new BuildKeyMappingPolicy(new NamedTypeBuildKey<string>());

            Assert.AreEqual(new NamedTypeBuildKey<string>(), policy.Map(new NamedTypeBuildKey<object>(), null));
        }
    }
}
