using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled
{
    [TestClass]
    public class BuildUp : Unity.Specification.Diagnostic.BuildUp.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Compiled);
        }
    }
}

namespace Resolved
{
    [TestClass]
    public class BuildUp : Unity.Specification.Diagnostic.BuildUp.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Activated);
        }
    }
}
