using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled
{

    [TestClass]
    public class Exceptions : Unity.Specification.Diagnostic.Exceptions.SpecificationTests
    {
        public override IUnityContainerAsync GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Compiled);
        }
    }
}

namespace Resolved
{

    [TestClass]
    public class Exceptions : Unity.Specification.Diagnostic.Exceptions.SpecificationTests
    {
        public override IUnityContainerAsync GetContainer()
        {
            return new UnityContainer(ModeFlags.Diagnostic | ModeFlags.Activated);
        }
    }
}
