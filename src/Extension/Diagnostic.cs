using Unity.Policy;

namespace Unity.Extension
{
    public class Diagnostic : UnityContainerExtension
    {
        protected override void Initialize()
        {
            ((UnityContainer)Container).SetDiagnosticPolicies();

            Context.ChildContainerCreated += (s, e) => ((UnityContainer)e.ChildContainer).SetDiagnosticPolicies(); 
        }

        public void ForceCompile()
        {
            ((UnityContainer)Container).Defaults.Set(typeof(ResolveDelegateFactory), 
                (ResolveDelegateFactory)((UnityContainer)Container).CompilingFactory);
        }

        public void DisableCompile()
        {
            ((UnityContainer)Container).Defaults.Set(typeof(ResolveDelegateFactory),
                (ResolveDelegateFactory)((UnityContainer)Container).ResolvingFactory);
        }
    }
}
