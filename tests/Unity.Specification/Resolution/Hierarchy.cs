using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Container.Hierarchy;

namespace Compiled
{
    [TestClass]
    public class Hierarchy : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Compiled);
        }
    }
}


namespace Resolved
{
    [TestClass]
    public class Hierarchy : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Activated);
        }
    }
}
