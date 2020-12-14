using System;

namespace Unity.Storage
{
    public class WeakDisposable : WeakReference, IDisposable
    {
        public WeakDisposable(IDisposable target)
            : base(target) { }

        public void Dispose() => (Target as IDisposable)?.Dispose();
    }
}
