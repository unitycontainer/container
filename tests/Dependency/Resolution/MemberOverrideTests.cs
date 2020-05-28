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
    public class FieldOverride : MemberOverrideTests<FieldInfo>
    {
        protected override ResolverOverride GetResolverOverride() => new Unity.Resolution.FieldOverride(string.Empty, OverrideValue);
        
        protected override ResolverOverride GetNamedResolverOverride() => new Unity.Resolution.FieldOverride(nameof(Name), new TestResolver());

        protected override FieldInfo GetMemberInfo() => typeof(ResolverOverrideTests).GetField(nameof(Name));

        [TestMethod]
        public void EqualsOverrideTest()
        {
            // Arrange
            var instance = GetNamedResolverOverride();

            // Validate
            Assert.IsTrue(instance.Equals((Unity.Resolution.FieldOverride)instance));
            Assert.IsFalse(instance.Equals((Unity.Resolution.FieldOverride)GetResolverOverride()));
        }

        [TestMethod]
        public void EqualsTargetedFieldTest()
        {
            // Arrange
            var instance = GetNamedResolverOverride().OnType<ResolverOverrideTests>();

            // Validate
            Assert.IsTrue(instance.Equals((Unity.Resolution.FieldOverride)instance));
            Assert.IsFalse(instance.Equals((Unity.Resolution.FieldOverride)GetResolverOverride().OnType<ResolverOverrideTests>()));
        }
    }


    [TestClass]
    public class PropertyOverride : MemberOverrideTests<PropertyInfo>
    {
        protected override ResolverOverride GetResolverOverride() => new Unity.Resolution.PropertyOverride(string.Empty, OverrideValue);

        protected override ResolverOverride GetNamedResolverOverride() => new Unity.Resolution.PropertyOverride(nameof(OverrideValue), new TestResolver());

        protected override PropertyInfo GetMemberInfo() => typeof(ResolverOverrideTests).GetProperty(nameof(OverrideValue));

        [TestMethod]
        public void EqualsOverrideTest()
        {
            // Arrange
            var instance = GetNamedResolverOverride();

            // Validate
            Assert.IsTrue(instance.Equals((Unity.Resolution.PropertyOverride)instance));
            Assert.IsFalse(instance.Equals(null));
            Assert.IsFalse(instance.Equals((Unity.Resolution.PropertyOverride)GetResolverOverride()));
        }

        [TestMethod]
        public void EqualsTargetedPropertyTest()
        {
            // Arrange
            var instance = GetNamedResolverOverride().OnType<ResolverOverrideTests>();

            // Validate
            Assert.IsTrue(instance.Equals((Unity.Resolution.PropertyOverride)instance));
            Assert.IsFalse(instance.Equals(null));
            Assert.IsFalse(instance.Equals((Unity.Resolution.PropertyOverride)GetResolverOverride().OnType<ResolverOverrideTests>()));
        }
    }
}
