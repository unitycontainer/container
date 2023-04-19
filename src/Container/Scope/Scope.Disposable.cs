using System.Runtime.CompilerServices;

namespace Unity.Container
{
    public abstract partial class Scope : ICollection<IDisposable>, 
                                          IDisposable
    {
        #region Collection

        public bool IsReadOnly => _disposables.IsReadOnly;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(IDisposable item) 
            => _disposables.Add(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() 
            => _disposables.Clear();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(IDisposable item)
            => _disposables.Contains(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(IDisposable[] array, int arrayIndex)
            => _disposables.CopyTo(array, arrayIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<IDisposable> GetEnumerator()
            => _disposables.GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(IDisposable item) 
            => _disposables.Remove(item);

        #endregion


        #region Dispose

        public virtual void Dispose()
        {
            IDisposable[] disposables;

            lock (_disposables)
            { 
                disposables = _disposables.ToArray();
                _disposables.Clear();
            }

            foreach (IDisposable disposable in disposables)
            {
                try
                {
                    disposable.Dispose();
                }
                catch { /* Ignore */ }
            }
        }

        #endregion
    }
}
