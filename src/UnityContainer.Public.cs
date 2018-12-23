using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Extension;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Extension Management

        /// <summary>
        /// Add an extension to the container.
        /// </summary>
        /// <param name="extension"><see cref="UnityContainerExtension"/> to add.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public IUnityContainer AddExtension(IUnityContainerExtensionConfigurator extension)
        {
            lock (_lifetimeContainer)
            {
                if (null == _extensions)
                    _extensions = new List<IUnityContainerExtensionConfigurator>();

                _extensions.Add(extension ?? throw new ArgumentNullException(nameof(extension)));
            }
            (extension as UnityContainerExtension)?.InitializeExtension(_context);

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
        public object Configure(Type configurationInterface)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            return _extensions?.FirstOrDefault(ex => configurationInterface.GetTypeInfo()
                                                                           .IsAssignableFrom(ex.GetType()
                                                                           .GetTypeInfo()));
#else
            return _extensions?.FirstOrDefault(ex => configurationInterface.IsAssignableFrom(ex.GetType()));
#endif
        }

        #endregion


        #region IDisposable Implementation

        /// <summary>
        /// Dispose this container instance.
        /// </summary>
        /// <remarks>
        /// Disposing the container also disposes any child containers,
        /// and disposes any instances whose lifetimes are managed
        /// by the container.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
