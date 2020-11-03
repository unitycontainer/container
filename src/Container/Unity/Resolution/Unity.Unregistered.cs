using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Unity.Container;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        private object? ResolveUnregistered(ref PipelineContext context)
        {
            var type = context.Contract.Type;
            if (!_policies.TryGet(type, out ResolveDelegate<PipelineContext>? pipeline))
                pipeline = _policies.AddOrGet(type, _policies.BuildPipeline(context.Contract.Type));

            // Resolve
            context.Target = pipeline!(ref context);
            
            if (!context.IsFaulted) context.LifetimeManager?.SetValue(context.Target, _scope);

            return context.Target;
        }
    }
}
