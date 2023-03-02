#if UNITY_V4
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;
#elif UNITY_V5 || UNITY_V6
using Unity;
using Unity.Builder;
using Unity.Extension;
#else
using Unity.Extension;
#endif


namespace Regression.Container
{
    public class BuilderAwareExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
#if UNITY_V4 || UNITY_V5 || UNITY_V6
            Context.Strategies.Add(new BuilderAwareStrategy(), UnityBuildStage.PostInitialization);
#else
            Context.TypePipelineChain.Add(UnityBuildStage.PostInitialization, new BuilderAwareStrategy());
#endif
        }
    }
}
