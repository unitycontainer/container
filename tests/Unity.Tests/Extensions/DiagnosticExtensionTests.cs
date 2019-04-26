using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity.Extension;

namespace Unity.Tests.v5
{
    [TestClass]
    public class DiagnosticExtensionTests
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
            var container = new UnityContainer();

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

        [TestMethod]
        public void DisposableExtensionsAreDisposedWithContainerButNotRemoved()
        {
            DisposableExtension extension = new DisposableExtension();
            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            container.Dispose();

            Assert.IsTrue(extension.Disposed);
            Assert.IsFalse(extension.Removed);
        }

        [TestMethod]
        public void OnlyDisposableExtensionAreDisposed()
        {
            DisposableExtension extension = new DisposableExtension();
            NoopExtension noop = new NoopExtension();

            IUnityContainer container = new UnityContainer()
                .AddExtension(noop)
                .AddExtension(extension);

            container.Dispose();

            Assert.IsTrue(extension.Disposed);
        }

        [TestMethod]
        public void CanSafelyDisposeContainerTwice()
        {
            DisposableExtension extension = new DisposableExtension();
            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            container.Dispose();
            container.Dispose();
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
        /// Add extension to the container. Check if object is returned. 
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

        private class DisposableExtension : UnityContainerExtension, IDisposable
        {
            public bool Disposed = false;
            public bool Removed = false;

            protected override void Initialize()
            {
            }

            public override void Remove()
            {
                this.Removed = true;
            }

            public void Dispose()
            {
                if (this.Disposed)
                {
                    throw new Exception("Can't dispose twice!");
                }
                this.Disposed = true;
            }
        }

        private class NoopExtension : UnityContainerExtension
        {
            protected override void Initialize()
            {
            }
        }
    }
}
