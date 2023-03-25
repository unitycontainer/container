using System;
using System.Runtime.CompilerServices;
using Unity.Builder;
using Unity.Resolution;

namespace Unity.Container
{
    public partial class FactoryStrategy<TContext> where TContext : IBuilderContext
    {
        public static ResolveDelegate<TContext> DefaultPipeline { get; }
            = (ref TContext context) => 
            {
                try
                {
                    var factory = context.Registration?.Factory;
                    if (factory is null)
                        context.Error("Invalid Factory");
                    else
                    {
                        if (context.Registration is Lifetime.PerResolveLifetimeManager)
                            context.PerResolve = factory(context.Container, context.Type, context.Name, context.Overrides);
                        else
                            context.Existing = factory(context.Container, context.Type, context.Name, context.Overrides);
                    }
                }
                catch (Exception ex)
                {
                    context.Capture(ex);
                }

                return context.Existing; 
            };


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BuilderStrategyDelegate(ref TContext context)
        {
            try
            {
                var factory = context.Registration?.Factory;
                if (factory is null)
                    context.Error("Invalid Factory");
                else
                {
                    if (context.Registration is Lifetime.PerResolveLifetimeManager)
                        context.PerResolve = factory(context.Container, context.Type, context.Name, context.Overrides);
                    else
                        context.Existing = factory(context.Container, context.Type, context.Name, context.Overrides);
                }
            }
            catch (Exception ex)
            {
                context.Capture(ex);
            }
        }
    }
}
