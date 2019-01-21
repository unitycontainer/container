using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Registration
{
    [TestClass]
    public class Native : Unity.Specification.Registration.Native.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }

    [TestClass]
    public class Extended : Unity.Specification.Registration.Extended.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }

    [TestClass]
    public class Syntax : Unity.Specification.Registration.Syntax.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
