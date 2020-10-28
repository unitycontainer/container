using System;
using System.Collections.Generic;

namespace Unity.Container
{
    public abstract partial class Scope : ICollection<IDisposable>, 
                                          IDisposable
    {
        #region Fields

        private bool _disposed;

        #endregion


        #region Collection

        public bool IsReadOnly
            => _disposables.IsReadOnly;

        public void Add(IDisposable item)
            => _disposables.Add(item);

        public void Clear()
            => _disposables.Clear();

        public bool Contains(IDisposable item)
            => _disposables.Contains(item);

        public void CopyTo(IDisposable[] array, int arrayIndex)
            => _disposables.CopyTo(array, arrayIndex);

        public IEnumerator<IDisposable> GetEnumerator()
            => _disposables.GetEnumerator();

        public bool Remove(IDisposable item) => _disposables.Remove(item);

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
