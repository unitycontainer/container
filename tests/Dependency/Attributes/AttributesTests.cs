using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Dependency.Attributes
{
    [TestClass]
    public class AttributesTests
    {
        private const string Name = "d32acfb9-5836-4357-bcb7-45a4ce6db395";

        [DataTestMethod]
        [DynamicData(nameof(DependencyResolutionAttributes), DynamicDataSourceType.Method)]
        public void DependencyAttributesTest(DependencyResolutionAttribute attribute)
        {
            Assert.IsNull(attribute.Name);
        }

        [DataTestMethod]
        [DynamicData(nameof(NamedAttributes), DynamicDataSourceType.Method)]
        public void NamedAttributesTest(DependencyResolutionAttribute attribute)
        {
            Assert.IsNotNull(attribute.Name);
        }

        #region Test Data

        public static IEnumerable<object[]> DependencyResolutionAttributes()
        {
            yield return new object[] { new DependencyAttribute() };
            yield return new object[] { new OptionalDependencyAttribute() };
        }

        public static IEnumerable<object[]> NamedAttributes()
        {
            yield return new object[] { new DependencyAttribute(Name) };
            yield return new object[] { new OptionalDependencyAttribute(Name) };
        }

        #endregion

    }
}
