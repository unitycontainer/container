using System;
using Unity.Container;
using Unity.Extension;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public static class DefaultPipelineFactory
    {
        private static Defaults? _policies;

        public static void Setup(ExtensionContext context)
        {
            _policies = (Defaults)context.Policies;
            _policies.Set(typeof(PipelineFactory), (PipelineFactory)Factory);
        }

        public static Pipeline Factory(ref ResolutionContext context)
        {
            return context.Manager switch
            {
                // Transient lifetime
                LifetimeManager { Style: ResolutionStyle.EveryTime }  => _policies!.OptimizedFactory(ref context),
                
                // Once in a while
                LifetimeManager { Style: ResolutionStyle.OnceInWhile } => _policies!.BalancedFactory(ref context),
                
                // Once in a lifetime
                LifetimeManager { Style: ResolutionStyle.OnceInLifetime } => context.Manager.Category switch
                {
                    RegistrationCategory.Type     => _policies!.TypePipeline,
                    RegistrationCategory.Instance => _policies!.InstancePipeline,
                    RegistrationCategory.Factory  => _policies!.FactoryPipeline,

                    _ => throw new NotImplementedException(),
                },

                // Unregistered type
                null => _policies!.UnregisteredFactory(ref context),

                // Not implemented
                _ => throw new NotImplementedException(),
            };
        }
    }
}
