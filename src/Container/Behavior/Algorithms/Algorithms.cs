using System;
using Unity.Builder;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.Container
{
    internal static partial class Algorithms<TContext>
        where TContext : IBuilderContext
    {
        #region Setup

        public static void Initialize(ExtensionContext context)
        {
            var policies = context.Policies;

            // Register default algorithms for registered and unregistered type resolutions
            policies.Set<ResolveDelegate<TContext>>(typeof(ContainerRegistration), RegisteredAlgorithm);
            policies.Set<ResolveDelegate<TContext>>(UnregisteredAlgorithm);
        }

        #endregion
    }
}
