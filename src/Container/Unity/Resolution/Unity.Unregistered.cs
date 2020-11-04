using System;
using Unity.Container;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        private object? ResolveUnregistered(ref Contract contract, ResolverOverride[] overrides)
        {
            object? value = default;
            var request = new RequestInfo(overrides);

            try
            {
                value = ResolveRequest(ref contract, ref request);
            }
            catch (Exception ex)
            {
                //if (context.Registration is SynchronizedLifetimeManager manager)
                //    manager.Recover();

                //context.Exception(ex);
            }

            //if (request.IsFaulted) throw new ResolutionFailedException(ref request);

            return value;
        }

        private object? ResolveSilent(ref Contract contract)
        {
            object? value = default;
#if NET45
            var request = new RequestInfo(new ResolverOverride[0]);
#else
            var request = new RequestInfo(Array.Empty<ResolverOverride>());
#endif
            try
            {
                value = ResolveRequest(ref contract, ref request);
            }
            catch
            {
                //if (context.Registration is SynchronizedLifetimeManager manager)
                //    manager.Recover();

                return null;
            }

            return request.IsFaulted ? null : value;
        }

        private object? ResolveUnregistered(ref PipelineContext context)
        {
            var type = context.Contract.Type;
            if (!_policies.TryGet(type, out ResolveDelegate<PipelineContext>? pipeline))
            {
                pipeline = _policies.BuildPipeline(context.Contract.Type);
                // TODO: pipeline = (ref PipelineContext c) => new object();
                pipeline = _policies.AddOrGet(type, pipeline);
            }

            // Resolve
            context.Target = pipeline!(ref context);
            
            if (!context.IsFaulted) context.LifetimeManager?.SetValue(context.Target, _scope);

            return context.Target;
        }
    }
}
