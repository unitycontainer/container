using System;
using System.Diagnostics;
using System.Threading;
using Unity.Builder;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        private ResolveDelegate<BuilderContext> PipelineFromRegistration(ref HashKey key, ExplicitRegistration registration, int position)
        {
            Debug.Assert(null != _registry);
            Debug.Assert(null != key.Type);

            var manager = registration.LifetimeManager;

            ResolveDelegate<BuilderContext>? pipeline = null;


            lock (_syncRegistry)
            {
                ref var entry = ref _registry.Entries[position];

                if (ReferenceEquals(entry.Registration, registration) && null == entry.Pipeline)
                {
                    entry.Pipeline = manager.Pipeline;
                    manager.PipelineDelegate = (ResolveDelegate<BuilderContext>)SpinWait;
                }
            }

            lock (manager)
            {
                if ((Delegate)(ResolveDelegate<BuilderContext>)SpinWait == manager.PipelineDelegate)
                {
                    PipelineBuilder builder = new PipelineBuilder(key.Type, registration);
                    manager.PipelineDelegate = builder.Pipeline();
                    pipeline = (ResolveDelegate<BuilderContext>)manager.PipelineDelegate;
                }
            }

            return manager.Pipeline;


            object? SpinWait(ref BuilderContext context)
            {
                while (null == pipeline)
                {
#if NETSTANDARD1_0 || NETCOREAPP1_0
                    for (var i = 0; i < 100; i++) ;
#else
                    Thread.SpinWait(100);
#endif
                }

                return pipeline(ref context);
            }
        }
    }
}
