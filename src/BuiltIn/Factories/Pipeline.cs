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
        private delegate void VisitorDelegate(ref PipelineContext context);

        private static Defaults? _policies;

        public static void Setup(ExtensionContext context)
        {
            _policies = (Defaults)context.Policies;
            _policies.Set<PipelineFactory>(typeof(Defaults.TypeCategory),     BuildTypePipeline);
            _policies.Set<PipelineFactory>(typeof(Defaults.InstanceCategory), BuildInstancePipeline);
            _policies.Set<PipelineFactory>(typeof(Defaults.FactoryCategory),  BuildFactoryPipeline);
        }

        private static ResolveDelegate<PipelineContext> BuildFactoryPipeline(Type type)
        {
            var pipeline = _policies!.FactoryPipeline;
            if (pipeline is not null) return pipeline;

            var processors = ((IEnumerable<BuilderStrategy>)_policies!.FactoryChain).ToArray();
            if (processors is null || 0 == processors.Length) throw new InvalidOperationException("List of visitors is empty");
            
            pipeline = (ref PipelineContext context) =>
            {
                var i = -1;

                while (!context.IsFaulted && ++i < processors.Length)
                    processors[i].PreBuildUp(ref context);

                while (!context.IsFaulted && --i >= 0)
                    processors[i].PostBuildUp(ref context);

                return context.Target;
            };

            _policies.Set<ResolveDelegate<PipelineContext>>(typeof(Defaults.FactoryCategory), pipeline);

            return pipeline;
        }

        private static ResolveDelegate<PipelineContext> BuildInstancePipeline(Type type)
        {
            var pipeline = _policies!.InstancePipeline;
            if (pipeline is not null) return pipeline;

            var processors = ((IEnumerable<BuilderStrategy>)_policies!.InstanceChain).ToArray();
            if (processors is null || 0 == processors.Length) throw new InvalidOperationException("List of visitors is empty");

            pipeline = (ref PipelineContext context) =>
            {
                var i = -1;

                while (!context.IsFaulted && ++i < processors.Length)
                    processors[i].PreBuildUp(ref context);

                while (!context.IsFaulted && --i >= 0)
                    processors[i].PostBuildUp(ref context);

                return context.Target;
            };

            _policies.Set<ResolveDelegate<PipelineContext>>(typeof(Defaults.InstanceCategory), pipeline);

            return pipeline;
        }

        private static ResolveDelegate<PipelineContext> BuildTypePipeline(Type type)
        {
            var pipeline = _policies!.TypePipeline;
            if (pipeline is not null) return pipeline;

            var processors = ((IEnumerable<BuilderStrategy>)_policies!.TypeChain).ToArray();
            if (processors is null || 0 == processors.Length) throw new InvalidOperationException("List of visitors is empty");


            pipeline = (ref PipelineContext context) =>
            {
                var i = -1;

                while (!context.IsFaulted && ++i < processors.Length)
                    processors[i].PreBuildUp(ref context);

                while (!context.IsFaulted && --i >= 0)
                    processors[i].PostBuildUp(ref context);

                return context.Target;
            };

            //_policies.Set<ResolveDelegate<PipelineContext>>(typeof(Defaults.TypeCategory), pipeline);

            return pipeline;
        }
    }
}
