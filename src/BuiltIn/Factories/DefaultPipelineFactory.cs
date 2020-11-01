using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Container;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public static class DefaultPipelineFactory
    {
        private static Defaults? _policies;

        public static void Setup(ExtensionContext context)
        {
            _policies = (Defaults)context.Policies;
            _policies.Set<PipelineFactory<PipelineContext>>(typeof(Defaults.TypeCategory),     BuildTypePipeline);
            _policies.Set<PipelineFactory<PipelineContext>>(typeof(Defaults.InstanceCategory), BuildInstancePipeline);
            _policies.Set<PipelineFactory<PipelineContext>>(typeof(Defaults.FactoryCategory),  BuildFactoryPipeline);
        }

        private static ResolveDelegate<PipelineContext> BuildFactoryPipeline(ref PipelineContext context)
        {
            var factoryProcessors = ((IEnumerable<PipelineProcessor>)_policies!.FactoryChain).ToArray();
            if (null == factoryProcessors || 0 == factoryProcessors.Length) throw new InvalidOperationException("List of visitors is empty");
            return (ref PipelineContext context) =>
            {
                var i = -1;

                while (!context.IsFaulted && ++i < factoryProcessors.Length)
                    factoryProcessors[i].PreBuild(ref context);

                while (!context.IsFaulted && --i >= 0)
                    factoryProcessors[i].PostBuild(ref context);

                return context.Target;
            };
        }

        private static ResolveDelegate<PipelineContext> BuildInstancePipeline(ref PipelineContext context)
        {
            var instanceProcessors = ((IEnumerable<PipelineProcessor>)_policies!.InstanceChain).ToArray();
            if (null == instanceProcessors || 0 == instanceProcessors.Length) throw new InvalidOperationException("List of visitors is empty");

            return (ref PipelineContext context) =>
            {
                var i = -1;

                while (!context.IsFaulted && ++i < instanceProcessors.Length)
                    instanceProcessors[i].PreBuild(ref context);

                while (!context.IsFaulted && --i >= 0)
                    instanceProcessors[i].PostBuild(ref context);

                return context.Target;
            };
        }

        private static ResolveDelegate<PipelineContext> BuildTypePipeline(ref PipelineContext context)
        {
            var typeProcessors = ((IEnumerable<PipelineProcessor>)_policies!.TypeChain).ToArray();
            if (null == typeProcessors || 0 == typeProcessors.Length) throw new InvalidOperationException("List of visitors is empty");

            return (ref PipelineContext context) =>
            {
                var i = -1;

                while (!context.IsFaulted && ++i < typeProcessors.Length)
                    typeProcessors[i].PreBuild(ref context);

                while (!context.IsFaulted && --i >= 0)
                    typeProcessors[i].PostBuild(ref context);

                return context.Target;
            };
        }
    }
}
