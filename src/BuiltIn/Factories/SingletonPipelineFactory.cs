using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Container;
using Unity.Extension;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public static class SingletonPipelineFactory
    {
        public static void Setup(ExtensionContext context)
        {
            var policies = (Defaults)context.Policies;
            var chain = (IEnumerable<PipelineProcessor>)context.TypePipelineChain;
            var processors = chain.ToArray();

            if (null == processors || 0 == processors.Length) throw new InvalidOperationException("List of visitors is empty");

            policies.Set(typeof(Defaults.TypeCategory), typeof(ResolveDelegate<PipelineContext>), (ResolveDelegate<PipelineContext>)Pipeline);


            object? Pipeline(ref PipelineContext context)
            {
                var i = -1;

                while (!context.IsFaulted && ++i < processors.Length)
                    processors[i].PreBuild(ref context);

                while (!context.IsFaulted && --i >= 0)
                    processors[i].PostBuild(ref context);

                return context.Target;
            }
        }
    }
}
