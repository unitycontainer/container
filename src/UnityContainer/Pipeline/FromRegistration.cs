using System;
using System.Diagnostics;
using Unity.Builder;
using Unity.Lifetime;
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

            lock (_syncRegistry)
            {
                ref var entry = ref _registry.Entries[position];
                if (ReferenceEquals(entry.Registration, registration)) entry.Pipeline = manager.Pipeline;
            }

            if (null == manager.PipelineDelegate)
            {
                PipelineBuilder builder = new PipelineBuilder(key.Type, registration);
                manager.PipelineDelegate = builder.Pipeline();
            }

            return manager.Pipeline;
        }
    }
}
