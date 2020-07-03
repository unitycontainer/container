using System.Diagnostics;

namespace Unity.Lifetime
{
    [DebuggerTypeProxy(typeof(LifetimeManagerProxy))]
    public abstract partial class LifetimeManager
    {
        protected class LifetimeManagerProxy
        {
            public LifetimeManagerProxy(LifetimeManager _)
            {

            }
        }
    }
}
