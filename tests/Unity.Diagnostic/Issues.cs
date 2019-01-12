using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Builder;
using Unity.Extension;

namespace GitHub
{
    [TestClass]
    public class Container : Unity.Specification.Diagnostic.Issues.Container.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled)
                .AddExtension(new Diagnostic())
                .AddExtension(new SpyExtension(new SpyStrategy(), UnityBuildStage.Initialization));
        }
    }

}
