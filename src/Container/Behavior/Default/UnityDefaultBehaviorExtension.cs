using Unity.Extension;
using Unity.Resolution;

namespace Unity.Container.Behavior.Default
{
    public partial class UnityDefaultBehaviorExtension : UnityContainerExtension
    {
        /// <summary>
        /// Install the default container behavior into the container.
        /// </summary>
        protected override void Initialize()
        {
        }
    }




    /// <summary>
    /// This extension supplies the default behavior of the UnityContainer API
    /// by handling the context events and setting policies.
    /// </summary>
    internal static partial class UnityDefaultBehaviorExtension<TContext>
        where TContext : IBuilderContext
    {
        /// <summary>
        /// Install the default container behavior into the container.
        /// </summary>
        public static void Initialize(ExtensionContext context)
        {
            // Initialize selection algorithms
            Selection.Initialize(context);

            // Default resolution algorithms
            Algorithms<TContext>.Initialize(context);

            // Pipeline Factories
            Pipelines.Initialize(context);

            // Add Type Factories
            Factories<TContext>.Initialize(context);
        }
    }
}
