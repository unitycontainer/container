using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Container;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public static class BalancedPipelineFactory
    {
        #region Fields

        public static MethodInfo FactoryMethodInfo = typeof(BalancedPipelineFactory).GetMethod(nameof(Factory))!;

        #endregion


        #region Scaffolding

        public static void Setup(ExtensionContext context)
        {
            var policies = (Defaults)context.Policies;
            var chain = (IEnumerable<PipelineProcessor>)context.TypePipelineChain;
            var processors = chain.Select(processor => (PipelineVisitor<Pipeline?>)processor.ResolutionVisitor).ToArray();

            if (0 == processors.Length) throw new InvalidOperationException("List of visitors is empty");

            policies.Set(typeof(Defaults.BalancedPipelineFactory), FactoryMethodInfo.CreateDelegate(typeof(Defaults.BalancedPipelineFactory), processors));

        }

        #endregion

        public static Pipeline Factory(PipelineVisitor<Pipeline?>[] visitors, ref ResolutionContext context) 
            => context.CreateBuilder(visitors).Build() ?? PipelineProcessor.DefaultPipeline;
    }
}
