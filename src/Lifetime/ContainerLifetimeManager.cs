using System;
using System.Diagnostics;

namespace Unity.Lifetime
{
    internal class ContainerLifetimeManager : LifetimeManager
    {
        protected override LifetimeManager OnCreateLifetimeManager()
            => throw new NotImplementedException();

        public override object GetValue(ILifetimeContainer container)
        {
            Debug.Assert(null != container);
            return container.Container;
        }

        public override object TryGetValue(ILifetimeContainer container)
        {
            Debug.Assert(null != container);
            return container.Container;
        }
    }
}
