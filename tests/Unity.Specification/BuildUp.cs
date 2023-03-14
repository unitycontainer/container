using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace BuildUp.Compiled
{
    [TestClass]
    public class BuildUp : Unity.Specification.BuildUp.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompillation());
        }
    }
}


namespace BuildUp.Activated
{
    [TestClass]
    public class BuildUp : Unity.Specification.BuildUp.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation());
        }
    }
}
