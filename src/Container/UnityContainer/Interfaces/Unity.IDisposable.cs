using Unity.Storage;

namespace Unity
{
    public sealed partial class UnityContainer : IDisposable
    {
        #region IDisposable

        private void Dispose(bool disposing)
        {
            // Explicit Dispose
            if (disposing)
            {
                _registering = null;
                _childContainerCreated = null;
            }

            ((LifetimeContainer)LifetimeContainer).Dispose();
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
