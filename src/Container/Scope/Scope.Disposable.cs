using System.Runtime.CompilerServices;
using Unity.Lifetime;

namespace Unity.Container
{
    public abstract partial class Scope : ILifetimeContainer, 
                                          IDisposable
    {
        #region Collection


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(IDisposable item) 
            => _disposables.Add(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(IDisposable item)
            => _disposables.Contains(item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(IDisposable item) 
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
