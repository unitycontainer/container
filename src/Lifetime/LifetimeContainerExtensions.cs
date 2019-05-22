using System;
using System.Collections.Generic;
using System.Text;

namespace Unity.Lifetime
{
    public static class LifetimeContainerExtensions
    {
        public static ILifetimeContainer GetRoot(this ILifetimeContainer container)
        {
            var root = container?.Container;
            while (root?.Parent != null)
                root = root.Parent;
            return (root as UnityContainer)?.LifetimeContainer;
        }
    }
}
