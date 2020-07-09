using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Extension;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Public Members

        public string? Name { get; }

        public UnityContainer Root { get; }

        public UnityContainer? Parent { get; }

        #endregion


        #region Extension Management

        /// <summary>
        /// Add an extension to the container.
        /// </summary>
        /// <param name="extension"><see cref="UnityContainerExtension"/> to add.</param>
        /// <returns>The <see cref="UnityContainer"/> that is being extended</returns>
        public UnityContainer AddExtension(UnityContainerExtension extension)
        {
            if (null == _context)
            { 
                lock (_scope)
                {
                    if (null == _context) _context = new PrivateExtensionContext(this);
                }
            }

            lock (_context)
            { 
                if (extension is IUnityContainerExtensionConfigurator configurator)
                { 
                    (_extensions ??= new List<IUnityContainerExtensionConfigurator>())
                        .Add(configurator);
                }

                extension?.InitializeExtension(_context);
            }

            return this;
        }

        /// <summary>
        /// Add an extension to the container.
        /// </summary>
        /// <param name="method"><see cref="Action{ExtensionContext}"/> delegate</param>
        /// <returns>The <see cref="UnityContainer"/> that is being extended</returns>
        public UnityContainer AddExtension(Action<ExtensionContext> method)
        {
            if (null == _context)
            {
                lock (_scope)
                {
                    if (null == _context) _context = new PrivateExtensionContext(this);
                }
            }

            lock (_context)
            { 
                method?.Invoke(_context ??= new PrivateExtensionContext(this));
            }

            return this;
        }

        /// <summary>
        /// Resolve access to a configuration interface exposed by an extension.
        /// </summary>
        /// <remarks>Extensions can expose configuration interfaces as well as adding
        /// strategies and policies to the container. This method walks the list of
        /// added extensions and returns the first one that implements the requested type.
        /// </remarks>
        /// <param name="configurationInterface"><see cref="Type"/> of configuration interface required.</param>
        /// <returns>The requested extension's configuration interface, or null if not found.</returns>
        public object? Configure(Type configurationInterface)
        {
            return _extensions?.FirstOrDefault(ex => configurationInterface.IsAssignableFrom(ex.GetType()));
        }

        #endregion


        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
