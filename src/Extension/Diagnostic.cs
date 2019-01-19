using Unity.Extension;

namespace Unity
{
    public class Diagnostic : UnityContainerExtension
    {
        protected override void Initialize()
        {
            ((UnityContainer)Container).SetDefaultPolicies = UnityContainer.SetDiagnosticPolicies;
            ((UnityContainer)Container).SetDefaultPolicies((UnityContainer)Container);
        }
    }
}
