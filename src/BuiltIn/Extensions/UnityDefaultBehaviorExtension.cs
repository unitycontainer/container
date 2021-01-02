using Unity.Container;

namespace Unity.Extension
{
    /// <summary>
    /// This extension supplies the default behavior of the UnityContainer API
    /// by handling the context events and setting policies.
    /// </summary>
    public class UnityDefaultBehaviorExtension
    {
        /// <summary>
        /// Install the default container behavior into the container.
        /// </summary>
        public static void Initialize(ExtensionContext context)
        {
            // Various selection predicates
            BuiltIn.Selectors.Setup(context);

            // Setup Built-In Factories
            BuiltIn.Factories.Setup(context);
        }
    }
}
