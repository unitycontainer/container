using System;
using Unity.Extension;

namespace Unity.Tests
{

    public class DisposableExtension : UnityContainerExtension, IDisposable
    {
        public bool Disposed = false;
        public bool Removed = false;

        protected override void Initialize()
        {
        }

        public void Dispose()
        {
            if (this.Disposed)
            {
                throw new Exception("Can't dispose twice!");
            }
            this.Disposed = true;
        }
    }
}
