using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace BuildUp.Compiled
{
    [TestClass]
    public class BuildUp : Unity.Specification.BuildUp.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompilation());
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



namespace BuildUp.Resolved
{
    [TestClass]
    public class BuildUp : Unity.Specification.BuildUp.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceResolution());
        }
    }
}
