using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Registration
{
    [TestClass]
    public class Types : Unity.Specification.Diagnostic.Registration.Types.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new Diagnostic());
        }
    }

    [TestClass]
    public class Instance : Unity.Specification.Diagnostic.Registration.Instance.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new Diagnostic());
        }
    }

    [TestClass]
    public class Factory : Unity.Specification.Diagnostic.Registration.Factory.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new Diagnostic());
        }
    }
}
