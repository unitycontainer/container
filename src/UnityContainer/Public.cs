using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Lifetime;

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
