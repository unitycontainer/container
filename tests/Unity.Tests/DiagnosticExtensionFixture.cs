using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
        public void ErrorMessage()
        {
            // Setup
            IUnityContainer container = new UnityContainer();

            // Validate
            try
            {
                container.Resolve<string>();
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
        }
    }
}
