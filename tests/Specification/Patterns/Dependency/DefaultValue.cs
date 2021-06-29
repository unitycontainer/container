using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;

namespace Dependency
{
    public abstract partial class Pattern
    {
        [PatternTestMethod("Dependency with default Value"), TestCategory(CATEGORY_DEPENDENCY)]
        [DynamicData(nameof(WithDefaultValue_Data))]
        public virtual void Import_WithDefault_Value(string test, Type type)
        {
            // Act
            var instance = Container.Resolve(type, null) as DependencyBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, type);
            Assert.AreEqual(instance.Default, instance.Value);

            // Arrange
            RegisterTypes();

            // Act
            instance = Container.Resolve(type, null) as DependencyBaseType;

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, type);
            Assert.AreEqual(Container.Resolve(instance.ImportType, null), instance.Value);
        }
    }
}
