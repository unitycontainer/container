using System;

namespace Unity.Container
{
    public abstract partial class Scope : IDisposable
    {
        #region Fields

        private bool _disposed;

        #endregion

        
        #region Finalizer

        ~Scope() => Dispose(false);

        #endregion


        #region Dispose

        public void Dispose() => Dispose(true);

        #endregion


        #region Implementation

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed) GC.SuppressFinalize(this);

            _disposed = true;
        }

        #endregion
    }
}
