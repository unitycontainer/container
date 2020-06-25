using System;
using System.Linq;
using Unity.Extension;

namespace Unity
{
    public partial class UnityContainer : IDisposable
    {
        #region Fields

        private bool disposedValue;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a <see cref="UnityContainer"/>.
        /// </summary>
        public UnityContainer()
        {
            // Extension Management
            _context = new ExtensionContextImpl(this);
        }

    #endregion


    #region Extension Management

    /// <summary>
    /// Add an extension to the container.
    /// </summary>
    /// <param name="extension"><see cref="UnityContainerExtension"/> to add.</param>
    /// <returns>The <see cref="IUnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
    public UnityContainer AddExtension(UnityContainerExtension extension)
        {
            _extensions.Add(extension ?? throw new ArgumentNullException(nameof(extension)));
            extension.InitializeExtension(_context);

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
            return _extensions.FirstOrDefault(ex => configurationInterface.IsAssignableFrom(ex.GetType()));
        }

        #endregion


        #region IDisposable Implementation

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~UnityContainer()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
