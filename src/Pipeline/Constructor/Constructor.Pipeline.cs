using System;
using System.Reflection;
using Unity.Container;
using Unity.Extension;

namespace Unity.Pipeline
{
    public partial class ConstructorProcessor : MethodBaseProcessor<ConstructorInfo>
    {


        #region Setup

        public static void SetupProcessor(ExtensionContext context)
        {
            // Create processor
            var processor = new ConstructorProcessor();

            // Add to pipeline chain
            context.TypePipeline.Add(processor, BuilderStage.Creation);

            // Subscribe to updates
            var policies = (Policies)context.Policies;

            policies.DefaultPolicyChanged += OnDefaultsChanged;
        }

        private static void OnDefaultsChanged(Type type, object? value)
        {
        }

        #endregion
    }
}
