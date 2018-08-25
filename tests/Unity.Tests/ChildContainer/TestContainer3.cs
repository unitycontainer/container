using System;

namespace Unity.Tests.v5.ChildContainer
{
    public class TestContainer3 : ITestContainer, IDisposable
    {
        private bool wasDisposed = false;

        public bool WasDisposed
        {
            get { return wasDisposed; }
            set { wasDisposed = value; }
        }
        
        public void Dispose()
        {
            wasDisposed = true;
        }
    }
}