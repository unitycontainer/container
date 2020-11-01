using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Container;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public static class BalancedPipelineFactory
    {
        #region Scaffolding

        public static void Setup(ExtensionContext context)
        {
            var policies = (Defaults)context.Policies;
            var chain = (IEnumerable<PipelineProcessor>)context.TypePipelineChain;
            var processors = chain.Select(processor => (PipelineVisitor<ResolveDelegate<PipelineContext>?>)processor.Build).ToArray();

            if (0 == processors.Length) throw new InvalidOperationException("List of visitors is empty");

            policies.Set(typeof(Defaults.BalancedPipelineFactory), (Defaults.BalancedPipelineFactory)Factory);
            
            ResolveDelegate<PipelineContext> Factory(ref PipelineContext context)
            {
                var builder = new PipelineBuilder<ResolveDelegate<PipelineContext>?>(ref context, processors);

                return builder.Build() ?? throw new InvalidOperationException("Invalid build chain");
            }
        }

        #endregion

    }
}
