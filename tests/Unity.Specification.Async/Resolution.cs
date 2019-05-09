using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled
{
    [TestClass]
    public class Basics : Unity.Specification.Resolution.Basics.SpecificationTests
    {
        public override IUnityContainerAsync GetContainer()
        {
            return new UnityContainer(ModeFlags.Compiled);
        }
    }
}


namespace Resolved
{
    [TestClass]
    public class Basics : Unity.Specification.Resolution.Basics.SpecificationTests
    {
        public override IUnityContainerAsync GetContainer()
        {
            return new UnityContainer(ModeFlags.Activated);
        }
    }
}
