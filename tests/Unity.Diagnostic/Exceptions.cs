using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled
{

    [TestClass]
    public class Exceptions : Unity.Specification.Diagnostic.Exceptions.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompillation())
                                       .AddExtension(new Diagnostic());
        }
    }
}

namespace Resolved
{

    [TestClass]
    public class Exceptions : Unity.Specification.Diagnostic.Exceptions.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceActivation())
                                       .AddExtension(new Diagnostic());
        }
    }
}
