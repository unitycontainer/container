using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Extension;

namespace Unity.Tests.v5
{
    [TestClass]
    public class DiagnosticExtension
    {
        [TestMethod]
        public void Register()
        {
            // Setup
            var container = new UnityContainer();

            // Act
            container.AddNewExtension<Diagnostic>();
            var config = container.Configure<Diagnostic>();

            // Validate
            Assert.IsNotNull(config);
        }


        [TestMethod]
        public void OptimizingResolve()
        {
            // Setup
            var container = new UnityContainer();

            // Validate
            Assert.IsNotNull(container.Resolve<object>());
        }

    }
}
