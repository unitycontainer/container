using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Registration
{
    [TestClass]
    public class Types : Unity.Specification.Async.Registration.Types.SpecificationTests
    {
        public override IUnityContainerAsync GetContainer()
        {
            return new UnityContainer();
        }
    }

    [TestClass]
    public class Factory : Unity.Specification.Async.Registration.Factory.SpecificationTests
    {
        public override IUnityContainerAsync GetContainer()
        {
            return new UnityContainer();
        }
    }

    [TestClass]
    public class Instance : Unity.Specification.Async.Registration.Instance.SpecificationTests
    {
        public override IUnityContainerAsync GetContainer()
        {
            return new UnityContainer();
        }
    }
}
