using Unity.Policy;

namespace Unity.Extension
{
    public class Diagnostic : UnityContainerExtension
    {
        protected override void Initialize()
        {
            ((UnityContainer)Container).ExecutePlan = UnityContainer.ValidatingExecutePlan;
        }

        public void ForceCompile()
        {
            ((UnityContainer)Container)._defaults.Set(typeof(ResolveDelegateFactory), 
                (ResolveDelegateFactory)((UnityContainer)Container).CompilingFactory);
        }

        public void DisableCompile()
        {
            ((UnityContainer)Container)._defaults.Set(typeof(ResolveDelegateFactory),
                (ResolveDelegateFactory)((UnityContainer)Container).ResolvingFactory);
        }
    }
}
