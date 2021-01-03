using Unity.Extension;

namespace Unity.Container
{
    internal static partial class Algorithms<TContext>
        where TContext : IBuilderContext
    {
        public static void Initialize(ExtensionContext context)
        {

            var policies = context.Policies;

            policies.Set<ResolveDelegate<TContext>>(typeof(ContainerRegistration),
                                                    RegisteredAlgorithm);

            policies.Set<ResolveDelegate<TContext>>(UnregisteredAlgorithm);
        }
    }
}
