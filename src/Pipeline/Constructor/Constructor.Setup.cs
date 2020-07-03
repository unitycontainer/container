using System;
using System.Reflection;
using Unity.Container;
using Unity.Extension;

namespace Unity.Pipeline
{
    public partial class ConstructorProcessor : MethodBaseProcessor<ConstructorInfo>
    {
        public static void SetupProcessor(ExtensionContext context)
        {
            // Create processor
            var processor = new ConstructorProcessor();

            // Add to pipeline chain
            context.TypePipeline.Add(processor, BuilderStage.Creation);

            // Subscribe to updates
            var defaults = (Defaults)context.Policies;

            defaults.DefaultPolicyChanged += OnDefaultsChanged;
        }

        private static void OnDefaultsChanged(Type type, object? value)
        {
        }
    }
}
