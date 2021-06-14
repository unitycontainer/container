using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif


namespace Resolution
{
    public partial class FromEmpty
    {
        [DataTestMethod, TestProperty(RESOLVING, nameof(FromEmpty))]
        [DynamicData(nameof(InvalidTypes_Data), typeof(PatternBase))]
        [ExpectedException(typeof(ResolutionFailedException))]
        public virtual void Unsupported(string test, Type type)
        {
            // Act
            _ = Container.Resolve(type, null);
        }

        [DataTestMethod, TestProperty(RESOLVING, nameof(FromEmpty))]
        [DynamicData(nameof(Unity_Recognized_Types_Data), typeof(PatternBase))]
        public virtual void Supported(string test, Type type)
        {
            // Act
            var instance = Container.Resolve(type, null);

            // Validate
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, type);
        }
    }
}
