using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Unity.Container
{
    public partial class ContainerScope : ICollection<IDisposable>, IDisposable
    {
        #region Fields

        private bool disposedValue;

        #endregion


        #region 

        int ICollection<IDisposable>.Count
            => _lifetimes.Count;

        bool ICollection<IDisposable>.IsReadOnly
            => _lifetimes.IsReadOnly;

        void ICollection<IDisposable>.Add(IDisposable item)
            => _lifetimes.Add(item);

        void ICollection<IDisposable>.Clear()
            => _lifetimes.Clear();

        bool ICollection<IDisposable>.Contains(IDisposable item)
            => _lifetimes.Contains(item);

        void ICollection<IDisposable>.CopyTo(IDisposable[] array, int arrayIndex)
            => _lifetimes.CopyTo(array, arrayIndex);

        bool ICollection<IDisposable>.Remove(IDisposable item)
            => _lifetimes.Remove(item);

        IEnumerator<IDisposable> IEnumerable<IDisposable>.GetEnumerator()
            => _lifetimes.GetEnumerator();

        #endregion


        #region IEnumerable

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion


        #region IDisposable

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
        // ~ContainerScope()
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
