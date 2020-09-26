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

        public static ResolveDelegate<PipelineContext> Factory(ref PipelineContext context)
        {
            return context.Registration?.Category switch
            {
                RegistrationCategory.Type => context.Registration switch
                {
                    // Every single time
                    LifetimeManager { Style: ResolutionStyle.EveryTime } => _policies!.TypePipeline/* _policies!.OptimizedFactory(ref context)*/,

                    // Once in a while
                    LifetimeManager { Style: ResolutionStyle.OnceInWhile } => _policies!.BalancedFactory(ref context),

                    // Once in a lifetime
                    LifetimeManager { Style: ResolutionStyle.OnceInLifetime } => _policies!.TypePipeline,
                    _ => throw new NotImplementedException(),
                },
                
                RegistrationCategory.Factory  => _policies!.FactoryPipeline,
                RegistrationCategory.Instance => _policies!.InstancePipeline,

                RegistrationCategory.Uninitialized => throw new NotImplementedException(),
                RegistrationCategory.Internal => throw new NotImplementedException(),
                RegistrationCategory.Clone => throw new NotImplementedException(),

                null => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
