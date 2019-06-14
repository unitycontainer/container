using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled
{

    [TestClass]
    public class Exceptions : Unity.Specification.Async.Exceptions.SpecificationTests
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
    public class Exceptions : Unity.Specification.Async.Exceptions.SpecificationTests
    {
        public override IUnityContainerAsync GetContainer()
        {
            return new UnityContainer(ModeFlags.Activated);
        }
    }
}
