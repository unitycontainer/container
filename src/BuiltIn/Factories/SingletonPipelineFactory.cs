using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Container;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public static class SingletonPipelineFactory
    {
        public static void Setup(ExtensionContext context)
        {
            var policies = (Defaults)context.Policies;

            var typeProcessors = ((IEnumerable<PipelineProcessor>)context.TypePipelineChain).ToArray();
            if (null == typeProcessors || 0 == typeProcessors.Length) throw new InvalidOperationException("List of visitors is empty");
            policies.Set(typeof(Defaults.TypeCategory), typeof(ResolveDelegate<PipelineContext>), (ResolveDelegate<PipelineContext>)TypePipeline);
            object? TypePipeline(ref PipelineContext context)
            {
                var i = -1;

                while (!context.IsFaulted && ++i < typeProcessors.Length)
                    typeProcessors[i].PreBuild(ref context);

                while (!context.IsFaulted && --i >= 0)
                    typeProcessors[i].PostBuild(ref context);

                return context.Target;
            }


            var instanceProcessors = ((IEnumerable<PipelineProcessor>)context.InstancePipelineChain).ToArray();
            if (null == instanceProcessors || 0 == instanceProcessors.Length) throw new InvalidOperationException("List of visitors is empty");
            policies.Set(typeof(Defaults.InstanceCategory), typeof(ResolveDelegate<PipelineContext>), (ResolveDelegate<PipelineContext>)InstancePipeline);
            object? InstancePipeline(ref PipelineContext context)
            {
                var i = -1;

                while (!context.IsFaulted && ++i < instanceProcessors.Length)
                    instanceProcessors[i].PreBuild(ref context);

                while (!context.IsFaulted && --i >= 0)
                    instanceProcessors[i].PostBuild(ref context);

                return context.Target;
            }



            var factoryProcessors = ((IEnumerable<PipelineProcessor>)context.FactoryPipelineChain).ToArray();
            if (null == factoryProcessors || 0 == factoryProcessors.Length) throw new InvalidOperationException("List of visitors is empty");
            policies.Set(typeof(Defaults.FactoryCategory), typeof(ResolveDelegate<PipelineContext>), (ResolveDelegate<PipelineContext>)FactoryPipeline);
            object? FactoryPipeline(ref PipelineContext context)
            {
                var i = -1;

                while (!context.IsFaulted && ++i < factoryProcessors.Length)
                    factoryProcessors[i].PreBuild(ref context);

                while (!context.IsFaulted && --i >= 0)
                    factoryProcessors[i].PostBuild(ref context);

                return context.Target;
            }
        }
    }
}
