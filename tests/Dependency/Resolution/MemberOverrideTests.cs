using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Unity.Resolution;

namespace Dependency.Overrides
{
    [TestClass]
    public abstract class MemberOverrideTests<TMemberInfo> : ResolverOverrideTests
        where TMemberInfo : MemberInfo
    {
        protected abstract TMemberInfo GetMemberInfo();

        #region IEquatable

        [TestMethod]
        public void EqualsInfoTest()
        {
            // Arrange
            var instance = GetNamedResolverOverride();

            // Validate
            Assert.IsTrue(instance.Equals(GetMemberInfo()));
        }

        [TestMethod]
        public void EqualsOverrideAsObjectTest()
        {
            // Arrange
            var instance = GetNamedResolverOverride();

            // Validate
            Assert.IsTrue(instance.Equals(instance));
            Assert.IsFalse(instance.Equals(null));
            Assert.IsFalse(GetResolverOverride().Equals(instance));
        }

        [TestMethod]
        public void EqualsTargetedAsObjectTest()
        {
            // Arrange
            var instance = GetNamedResolverOverride().OnType<ResolverOverrideTests>();

            // Validate
            Assert.IsTrue(instance.Equals(instance));
            Assert.IsFalse(instance.Equals(null));
            Assert.IsFalse(GetResolverOverride().OnType<ResolverOverrideTests>().Equals(instance));
        }

        #endregion
    }


    [TestClass]
    public class FieldOverrideTests : MemberOverrideTests<FieldInfo>
    {
        protected override ResolverOverride GetResolverOverride() => new FieldOverride(string.Empty, OverrideValue);
        
        protected override ResolverOverride GetNamedResolverOverride() => new FieldOverride(nameof(Name), new TestResolver());

        protected override FieldInfo GetMemberInfo() => typeof(ResolverOverrideTests).GetField(nameof(Name));

        [TestMethod]
        public void EqualsOverrideTest()
        {
            // Arrange
            var instance = GetNamedResolverOverride();

            // Validate
            Assert.IsTrue(instance.Equals((FieldOverride)instance));
            Assert.IsFalse(instance.Equals((FieldOverride)GetResolverOverride()));
        }

        [TestMethod]
        public void EqualsTargetedFieldTest()
        {
            // Arrange
            var instance = GetNamedResolverOverride().OnType<ResolverOverrideTests>();

            // Validate
            Assert.IsTrue(instance.Equals((FieldOverride)instance));
            Assert.IsFalse(instance.Equals((FieldOverride)GetResolverOverride().OnType<ResolverOverrideTests>()));
        }
    }


    [TestClass]
    public class PropertyOverrideTests : MemberOverrideTests<PropertyInfo>
    {
        protected override ResolverOverride GetResolverOverride() => new PropertyOverride(string.Empty, OverrideValue);

        protected override ResolverOverride GetNamedResolverOverride() => new PropertyOverride(nameof(OverrideValue), new TestResolver());

        protected override PropertyInfo GetMemberInfo() => typeof(ResolverOverrideTests).GetProperty(nameof(OverrideValue));

        [TestMethod]
        public void EqualsOverrideTest()
        {
            // Arrange
            var instance = GetNamedResolverOverride();

            // Validate
            Assert.IsTrue(instance.Equals((PropertyOverride)instance));
            Assert.IsFalse(instance.Equals(null));
            Assert.IsFalse(instance.Equals((PropertyOverride)GetResolverOverride()));
        }

        [TestMethod]
        public void EqualsTargetedPropertyTest()
        {
            // Arrange
            var instance = GetNamedResolverOverride().OnType<ResolverOverrideTests>();

            // Validate
            Assert.IsTrue(instance.Equals((PropertyOverride)instance));
            Assert.IsFalse(instance.Equals(null));
            Assert.IsFalse(instance.Equals((PropertyOverride)GetResolverOverride().OnType<ResolverOverrideTests>()));
        }
    }
}
