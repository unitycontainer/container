﻿using System.Runtime.CompilerServices;
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
                            context.Existing = factory(context.Container, context.Type, context.Name);
                        else
                            context.Existing = factory(context.Container, context.Type, context.Name);
                    }
                }
                catch (Exception ex)
                {
                    context.Capture(ex);
                }

                return context.Existing; 
            };


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FactoryBuilderStrategy(ref TContext context)
        {
            try
            {
                var factory = context.Registration?.Factory;
                if (factory is null)
                    context.Error("Invalid Factory");
                else
                {
                    if (context.Registration is Lifetime.PerResolveLifetimeManager)
                        context.Existing = factory(context.Container, context.Type, context.Name);
                    else
                        context.Existing = factory(context.Container, context.Type, context.Name);
                }
            }
            catch (Exception ex)
            {
                context.Capture(ex);
            }
        }
    }
}