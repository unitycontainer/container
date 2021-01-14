using System;
using Unity.Extension;
using Unity.Lifetime;


namespace Unity.Container
{
    internal static partial class Algorithms<TContext>
    {
        /// <summary>
        /// Default algorithm for resolution of registered types
        /// </summary>
        public static object? RegisteredAlgorithm(ref TContext context)
        {
            var manager = context.Registration!;

            // Double lock check and create pipeline
            var pipeline = manager.GetPipeline<TContext>(context.Container.Scope);
            if (pipeline is null)
            {
                lock (manager)
                { 
                    if ((pipeline = manager.GetPipeline<TContext>(context.Container.Scope)) is null)
                    {
                        switch (manager.Category)
                        {
                            case RegistrationCategory.Factory:
                                pipeline = ((Policies<TContext>)context.Policies).FactoryPipeline;
                                break;

                            case RegistrationCategory.Instance:
                                pipeline = ((Policies<TContext>)context.Policies).InstancePipeline;
                                break;

                            case RegistrationCategory.Type:

                                if (!manager.RequireBuild && context.Contract.Type != manager.Type)
                                {
                                    // Type Mapping
                                    var contract = new Contract(manager.Type!, context.Contract.Name);
                                    pipeline = (ref TContext c) => c.MapTo(contract);
                                }
                                else
                                { 
                                    pipeline = PipelineFactory(ref context);
                                }
                                break;
                            
                            default:
                                throw new NotSupportedException();
                        }

                        manager.SetPipeline(context.Container.Scope, pipeline);
                    }
                }
            }

            // Resolve
            pipeline(ref context);

            // Handle errors, if any
            if (context.IsFaulted)
            {
                (manager as SynchronizedLifetimeManager)?.Recover();
                return UnityContainer.NoValue;
            }

            // Save resolved value
            manager.SetValue(context.Existing, context.Container.Scope);

            return context.Existing;
        }
    }
}
