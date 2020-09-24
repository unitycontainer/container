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
            var chain = (IEnumerable<Container.PipelineProcessor>)context.TypePipelineChain;
            var processors = chain.Select(processor => (PipelineVisitor<object?>)processor.Build).ToArray();

            if (0 == processors.Length) throw new InvalidOperationException("List of visitors is empty");

            policies.Set(typeof(Defaults.TypeCategory), typeof(ResolveDelegate<PipelineContext>), PipelineMethodInfo.CreateDelegate(typeof(ResolveDelegate<PipelineContext>), processors));
        }


        #endregion


        public static object? Pipeline(PipelineVisitor<object?>[] visitors, ref PipelineContext context)
        {
            var builder = new PipelineBuilder<object?>(ref context, visitors);

            return builder.Build() ?? throw new InvalidOperationException("Invalid build chain");
        }
    }
}
