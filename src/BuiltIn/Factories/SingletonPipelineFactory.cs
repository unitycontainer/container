using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Container;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public static class SingletonPipelineFactory
    {
        #region Fields

        public static MethodInfo PipelineMethodInfo   = typeof(SingletonPipelineFactory).GetMethod(nameof(Pipeline))!;

        #endregion


        #region Scaffolding

        public static void Setup(ExtensionContext context)
        {
            var policies = (Defaults)context.Policies;
            var chain = (IEnumerable<PipelineProcessor>)context.TypePipelineChain;
            var processors = chain.Select(processor => (PipelineVisitor<object?>)processor.BuildUpVisitor).ToArray();

            if (0 == processors.Length) throw new InvalidOperationException("List of visitors is empty");

            policies.Set(typeof(Defaults.TypeCategory), typeof(Pipeline), PipelineMethodInfo.CreateDelegate(typeof(Pipeline), processors));
        }


        #endregion


        public static void Pipeline(PipelineVisitor<object?>[] visitors, ref ResolutionContext context) 
            => context.CreateBuilder(visitors).Build();
    }
}
