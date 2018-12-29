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
        /// Create instance of Diagnostic and add it to the UnityContainer
        /// </summary>
        [TestMethod]
        public void AddExistingMyCustonExtensionToContainer()
        {
            Diagnostic extension = new Diagnostic();
            IUnityContainer uc = new UnityContainer();
            uc.AddExtension(extension);

            Assert.IsNotNull(uc);
            // TODO: Implement - Assert.IsTrue(extension.CheckPolicyValue);
        }

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
        /// Check whether extension is added to the container created.
        /// </summary>
        [TestMethod]
        public void CheckExtensionAdded()
        {
            Diagnostic extension = new Diagnostic();
            IUnityContainer uc = new UnityContainer();
            uc.AddExtension(extension);

            // TODO: Implement - Assert.IsTrue(extension.CheckExtensionValue);
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
        /// GetOrDefault access to the configuration interface given by the extension.
        /// </summary>
        [TestMethod]
        public void ConfigureToContainer()
        {
            Diagnostic extension = new Diagnostic();

            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            // TODO: Implement - IMyCustomConfigurator extend = container.Configure<IMyCustomConfigurator>();

            // TODO: Implement - Assert.AreSame(extension.Container, extend.Container);
        }

        // TODO: Implement - 
        ///// <summary>
        ///// Add new extension, access the configurator exposed by extension, add a new policy.
        ///// </summary>
        //[TestMethod]
        //public void AddExtensionAddPolicyAddConfigurator()
        //{
        //    IUnityContainer container = new UnityContainer()
        //         .AddNewExtension<Diagnostic>()
        //         .Configure<IMyCustomConfigurator>()
        //             .AddPolicy().Container;

        //    Assert.IsNotNull(container);
        //}

        ///// <summary>
        ///// Add existing instance of extension, access the configurator exposed by extension, add a new policy.
        ///// </summary>
        //[TestMethod]
        //public void AddExistExtensionAddPolicyAddConfigurator()
        //{
        //    Diagnostic extension = new Diagnostic();
        //    IUnityContainer container = new UnityContainer()
        //         .AddExtension(extension)
        //         .Configure<IMyCustomConfigurator>()
        //             .AddPolicy().Container;

        //    Assert.IsNotNull(container);
        //    Assert.IsTrue(extension.CheckPolicyValue);
        //}

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