using System;
using System.Reflection;
using Unity.Container;
using Unity.Extension;

namespace Unity.Pipeline
{
    public partial class PropertyProcessor : MemberProcessor<PropertyInfo, object>
    {
        public static void SetupProcessor(ExtensionContext context)
        {
            // Create processor
            var processor = new PropertyProcessor();

            // Add to pipeline chain
            context.TypePipeline.Add(BuilderStage.Properties, processor);

            // Subscribe to updates
            var defaults = (Defaults)context.Policies;

            defaults.DefaultPolicyChanged += OnDefaultsChanged;
        }

        private static void OnDefaultsChanged(Type type, object? value)
        {
        }
    }
}
