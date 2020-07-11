using System;

namespace Unity.Container
{
    public partial class ContainerScope : IDisposable
    {
        public void Dispose() => Dispose(disposing: true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) GC.SuppressFinalize(this);

            _poolMeta.Return(_registryMeta);
            _poolMeta.Return(_contractMeta);
        }

    }
}
