using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Extension;

namespace Unity.Tests.v5.Extension
{
    /// <summary>
    /// Summary description for DiagnosticFixture
    /// </summary>
    [TestClass]
    public class DiagnosticFixture
    {
        /// <summary>
        /// Add the Diagnostic to the UnityContainer
        /// </summary>
        [TestMethod]
        public void AddMyCustonExtensionToContainer()
        {
            IUnityContainer uc = new UnityContainer();
            uc.AddNewExtension<Diagnostic>();

            Assert.IsNotNull(uc);
        }

        /// <summary>
        /// Check whether extension is added to the container created.
        /// </summary>
        [TestMethod]
        public void CheckExtensionAddedToContainer()
        {
            Diagnostic extension = new Diagnostic();
            IUnityContainer uc = new UnityContainer();
            uc.AddExtension(extension);

            Assert.AreSame(uc, extension.Container);
        }

        /// <summary>
        /// Add extension to the conatiner. Check if object is returned. 
        /// </summary>
        [TestMethod]
        public void AddExtensionGetObject()
        {
            Diagnostic extension = new Diagnostic();

            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            object result = container.Resolve<object>();

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Remove all extensions. Add default extension and the new extension.
        /// </summary>
        [TestMethod]
        public void AddDefaultAndCustomExtensions()
        {
            IUnityContainer container = new UnityContainer()
                .AddExtension(new Diagnostic());

            object result = container.Resolve<object>();

            Assert.IsNotNull(result);
            Assert.IsNotNull(container);
        }

        /// <summary>
        /// Add existing instance of extension. SetLifetime of the extension with the container.
        /// </summary>       
        [TestMethod]
        public void AddExtensionSetLifetime()
        {
            Diagnostic extension = new Diagnostic();
            IUnityContainer container = new UnityContainer()
                 .AddExtension(extension);

            container.RegisterInstance<Diagnostic>(extension);
            object result = container.Resolve<object>();
            
            Assert.IsNotNull(result);
            Assert.IsNotNull(container);
        }
    }
}