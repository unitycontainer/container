using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Resolution.Enumerable;

namespace Compiled
{
    [TestClass]
    public class Enumerable : SpecificationTests
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
    public class Enumerable : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(ModeFlags.Activated);
        }
    }
}
