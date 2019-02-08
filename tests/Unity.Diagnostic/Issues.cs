using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Builder;
using Unity.Specification.Diagnostic.Issues.GitHub;

namespace GitHub
{
    [TestClass]
    public class Container : Unity.Specification.Diagnostic.Issues.GitHub.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled)
                .AddExtension(new Diagnostic())
                .AddExtension(new SpyExtension(new SpyStrategy(), UnityBuildStage.Initialization));
        }
    }

}
