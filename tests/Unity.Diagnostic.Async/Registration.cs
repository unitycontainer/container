using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Registration
{
    [TestClass]
    public class Types : Unity.Specification.Diagnostic.Async.Registration.Types.SpecificationTests
    {
        public override IUnityContainerAsync GetContainer()
        {
            return new UnityContainer(true);
        }
    }

    [TestClass]
    public class Factory : Unity.Specification.Diagnostic.Async.Registration.Factory.SpecificationTests
    {
        public override IUnityContainerAsync GetContainer()
        {
            return new UnityContainer(true);
        }
    }

    [TestClass]
    public class Instance : Unity.Specification.Diagnostic.Async.Registration.Instance.SpecificationTests
    {
        public override IUnityContainerAsync GetContainer()
        {
            return new UnityContainer(true);
        }
    }
}
