using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Extension;

namespace Unity.Tests.v5
{
    [TestClass]
    public class LegacyExtensionTests
    {
        [TestMethod]
        public void Register()
        {
            // Setup
            var container = new UnityContainer();
            container.AddNewExtension<Legacy>();

            // Act
            var config = container.Configure<Legacy>();

            // Validate
            Assert.IsNotNull(config);
        }

        [TestMethod]
        public void SmartByDefault()
        {
            // Setup
            var container = new UnityContainer();

            // Act
            var result = container.Resolve<ObjectWithMultipleConstructors>();

            // Validate
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void LegacySelection()
        {
            // Setup
            var container = new UnityContainer();
            container.AddNewExtension<Legacy>();

            // Act
            container.Resolve<ObjectWithMultipleConstructors>();
        }
    }

    #region Test Data

    public class ObjectWithMultipleConstructors
    {
        public const string One = "1";
        public const string Two = "2";
        public const string Three = "3";
        public const string Four = "4";
        public const string Five = "5";

        public string Signature { get; }

        public ObjectWithMultipleConstructors(int first)
        {
            Signature = One;
        }

        public ObjectWithMultipleConstructors(object first, IUnityContainer second)
        {
            Signature = Two;
        }

        public ObjectWithMultipleConstructors(object first, string second, IUnityContainer third)
        {
            Signature = Three;
        }
    }

    #endregion
}
