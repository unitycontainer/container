using System;
using System.Reflection;
using Unity.Container;
using Unity.Extension;

namespace Unity.Pipeline
{
    public class PropertyProcessor : MemberProcessor<PropertyInfo, object>
    {

        #region Setup

        public static void SetupProcessor(ExtensionContext context)
        {
            // Create processor
            var processor = new PropertyProcessor();

            // Add to pipeline chain
            context.TypePipeline.Add(processor, BuilderStage.Fields);

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
